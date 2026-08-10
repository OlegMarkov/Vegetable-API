using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Vegetable.Core.Database;
using Vegetable.Core.Services;
using Vegetable.Entities;

namespace Vegetable.Workers
{
    public class NotificationMessagesSendWorker : BackgroundService
    {
        private readonly ILogger<NotificationMessagesSendWorker> _logger;
        private readonly INotificationMessageRepo _notificationMessageRepo;
        private readonly IPushService _pushService;
        private readonly IConfiguration _configuration;
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly int _sleepSeconds;

        public NotificationMessagesSendWorker(ILogger<NotificationMessagesSendWorker> logger, IServiceProvider serviceProvider, IPushService pushService, IConfiguration configuration, ITelegramBotClient telegramBotClient)
        {
            _logger = logger;
            _notificationMessageRepo = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<INotificationMessageRepo>();
            _pushService = pushService;
            _configuration = configuration.GetSection("Workers");
            _telegramBotClient = telegramBotClient;
            _sleepSeconds = int.Parse(_configuration["NotificationMessagesSendWorkerSleepSeconds"]);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("NotificationsWorker running at: {time}", DateTimeOffset.Now);
                var count = 0;
                var notifications = await _notificationMessageRepo.GetNotificationMessagesForSending();

                while (notifications.Length > 0)
                {
                    foreach (var notification in notifications)
                    {
                        var result = await SendNotification(notification);
                        if (result.IsSuccess)
                        {
                            notification.SentDateUTC = DateTime.UtcNow;
                        }
                        else
                        {
                            notification.SendAttempts += 1;
                        }
                        notification.Result = result.Result;
                        await _notificationMessageRepo.UpdateNotificationMessage(notification);
                    }
                    count += notifications.Length;
                    notifications = await _notificationMessageRepo.GetNotificationMessagesForSending();
                }

                _logger.LogInformation("NotificationMessagesSendWorker done at: {time}, total records: {count}", DateTimeOffset.Now, count);
                await Task.Delay(TimeSpan.FromSeconds(_sleepSeconds), stoppingToken);
            }
        }

        private async Task<NotificationResult> SendNotification(NotificationMessage notification)
        {
            var result = new NotificationResult { IsSuccess = true };
            switch (notification.Channel)
            {
                case NotificationChannel.Push:
                    try
                    {
                        result.Result = await _pushService.PushMessageToSingleAsync(notification.Recipient, notification.Title, notification.Text, notification.RedirectUrl, notification.Platform);
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.Result = ex.Message;
                    }
                    break;
                case NotificationChannel.Telegram:
                    try
                    {
                        var response = await _telegramBotClient.SendTextMessageAsync(notification.Recipient, notification.Text);
                        result.IsSuccess = response.MessageId != 0;
                        result.Result = $"MessageId: {response.MessageId}";
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.Result = ex.Message;
                    }
                    break;
                case NotificationChannel.TelegramInlineKeyboard:
                    try
                    {
                        var buttons = JsonSerializer.Deserialize<List<InlineKeyboardButton>>(notification.RedirectUrl);
                        InlineKeyboardMarkup inlineKeyboard = new(buttons);
                        var response = await _telegramBotClient.SendTextMessageAsync(
                            notification.Recipient, notification.Text, replyMarkup: inlineKeyboard);
                        result.IsSuccess = response.MessageId != 0;
                        result.Result = $"MessageId: {response.MessageId}";
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.Result = ex.Message;
                    }
                    break;
                case NotificationChannel.SMS:
                    break;
                default:
                    break;
            }
            return result;
        }

        struct NotificationResult
        {
            public bool IsSuccess { get; set; }
            public string Result { get; set; }
        }
    }
}
