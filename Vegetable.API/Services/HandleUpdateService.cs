using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using Vegetable.Core.Database;
using Vegetable.Core.Services;
using Vegetable.Core.Storage;
using Vegetable.Entities;
using static Vegetable.Core.Storage.Models.BotCommand;

namespace Vegetable.API.Services
{
    public class HandleUpdateService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<HandleUpdateService> _logger;
        private readonly IBotCommandRepository _botCommandRepository;
        private readonly INotificationsService _notificationsService;
        private readonly IOwnerRepo _repo;
        private readonly IConfiguration _translations;

        public HandleUpdateService(ITelegramBotClient botClient, ILogger<HandleUpdateService> logger, IBotCommandRepository botCommandRepository, IOwnerRepo repo, IConfiguration configuration, INotificationsService notificationsService)
        {
            _botClient = botClient;
            _logger = logger;
            _botCommandRepository = botCommandRepository;
            _repo = repo;
            _translations = configuration.GetSection("Translations:Bot");
            _notificationsService = notificationsService;
        }

        public async Task EchoAsync(Update update)
        {
            var handler = update.Type switch
            {
                UpdateType.Message => BotOnMessageReceived(update.Message!),
                UpdateType.EditedMessage => BotOnMessageReceived(update.EditedMessage!),
                UpdateType.CallbackQuery => BotOnCallbackQueryReceived(update.CallbackQuery!),
                UpdateType.MyChatMember => BotOnChatMemberChanges(update.MyChatMember!),
                _ => UnknownUpdateHandlerAsync(update)
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(exception);
            }
        }

        private async Task BotOnChatMemberChanges(ChatMemberUpdated chatMemberUpdated)
        {
            if (chatMemberUpdated == null) return;
            if (chatMemberUpdated.NewChatMember.Status == ChatMemberStatus.Kicked)
            {
                await _repo.RemoveChatIdFromCustomers(chatMemberUpdated.From.Id);
            }
        }

        private async Task BotOnMessageReceived(Message message)
        {
            _logger.LogInformation("Receive message type: {messageType}", message.Type);
            if (message.Type != MessageType.Text)
                return;
            var language = GetLanguage(message.From);
            var action = message.Text!.Split(' ')[0] switch
            {
                "/start" => Start(_botClient, message),
                _ => Usage(_botClient, message)
            };
            Message sentMessage = await action;
            if (sentMessage != null)
                _logger.LogInformation("The message was sent with id: {sentMessageId}", sentMessage.MessageId);

            async Task<Message> Start(ITelegramBotClient bot, Message message)
            {
                var words = message.Text!.Split(' ');
                
                if (words.Length > 1) {
                    var command = _botCommandRepository.GetCommand(words[1]);
                    if (command != null)
                    {
                        return await HandleCommand(message, command);
                    }
                    else
                    {
                        return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
                                                      text: _translations["InvalidLink." + language],
                                                      replyMarkup: new ReplyKeyboardRemove());
                    }
                }
                else {
                    return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
                                                   text: _translations["ContactOwner." + language],
                                                   replyMarkup: new ReplyKeyboardRemove());
                }
            }

            async Task<Message> Usage(ITelegramBotClient bot, Message message)
            {
                return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
                                                      text: _translations["DefaultAnswer." + language],
                                                      replyMarkup: new ReplyKeyboardRemove());
            }
        }


        private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery)
        {
            var command = _botCommandRepository.GetCommand(callbackQuery.Data);
            await _botClient.EditMessageReplyMarkupAsync(chatId: callbackQuery.Message!.Chat.Id, messageId: callbackQuery.Message.MessageId, InlineKeyboardMarkup.Empty());
            if (command != null)
            {
                await HandleCommand(callbackQuery.Message!, command);
            }
            else
            {
                var language = GetLanguage(callbackQuery.From!);
                await _botClient.SendTextMessageAsync(chatId: callbackQuery.Message!.Chat.Id,
                                              text: _translations["InvalidLink." + language],
                                              replyMarkup: new ReplyKeyboardRemove());
            }
        }

        private async Task<Message> HandleCommand(Message message, Core.Storage.Models.BotCommand command)
        {
            var language = GetLanguage(message.From);
            switch (command.Type)
            {
                case CommandType.Subscribe:
                    {
                        if (await _repo.UpdateCustomerChatId(command.GetPayload<Guid>(), message.Chat.Id, language))
                        {
                            _botCommandRepository.RemoveCommand(command.Key);
                            return await _botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                          text: _translations["Subscribed." + language],
                                                          replyMarkup: new ReplyKeyboardRemove());
                        }
                        else
                        {
                            return await _botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                          text: _translations["CustomerNotFound." + language],
                                                          replyMarkup: new ReplyKeyboardRemove());
                        }
                    }
                case CommandType.ConfirmReservation:
                case CommandType.SubscribeWithReservation:
                    {
                        var reservation = command.GetPayload<Reservation>();
                        if (command.Type == CommandType.SubscribeWithReservation)
                        {
                            reservation.Customer.ChatId = message.Chat.Id;
                            reservation.Customer.ChatLanguage = language;
                        }
                        var owner = await _repo.GetOwner(reservation.OwnerId, true);
                        var reservationInRange = await _repo.GetReservationsByTimeRange(owner.Id, reservation.StartTime, reservation.EndTime, owner.TimeZone);
                        if (reservationInRange != null && reservationInRange.Any())
                            return await _botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                          text: _translations["TimeUnavalible." + language],
                                                          replyMarkup: new ReplyKeyboardRemove());

                        var newReservation = await _repo.CreateReservation(reservation.OwnerId, reservation);
                        await _notificationsService.ReservationCreated(owner.Id, newReservation);
                        _botCommandRepository.RemoveCommand(command.Key);
                        return await Task.FromResult<Message>(null);
                    }
                default:
                    return await _botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                          text: _translations["UnknownCommand." + language],
                                                          replyMarkup: new ReplyKeyboardRemove());
            }
        }

        private Task UnknownUpdateHandlerAsync(Update update)
        {
            _logger.LogInformation("Unknown update type: {updateType}", update.Type);
            return Task.CompletedTask;
        }

        public Task HandleErrorAsync(Exception exception)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            _logger.LogInformation("HandleError: {ErrorMessage}", ErrorMessage);
            return Task.CompletedTask;
        }

        private string GetLanguage(Telegram.Bot.Types.User from)
        {
            return (from.LanguageCode != null && (from.LanguageCode == "ru" || from.LanguageCode == "en")) ? from.LanguageCode : "en";
        }
    }
}
