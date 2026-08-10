using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using Vegetable.Core.Database;
using Vegetable.Core.Extensions;
using Vegetable.Entities;

namespace Vegetable.Core.Services
{
    public class NotificationsService : INotificationsService
    {
        private readonly IOwnerRepo _ownerRepo;
        private readonly IConfiguration _translations;
        private readonly INotificationMessageRepo _notificationMessageRepo;

        public NotificationsService(IOwnerRepo ownerRepo, IConfiguration configuration, INotificationMessageRepo notificationMessageRepo)
        {
            _ownerRepo = ownerRepo;
            _translations = configuration.GetSection("Translations:Notifications");
            _notificationMessageRepo = notificationMessageRepo;
        }

        public async Task ReservationCreated(Guid ownerId, Reservation reservation)
        {
            if (reservation.StartTime < DateTime.UtcNow) return;

            var owner = await _ownerRepo.GetOwner(ownerId, true);
            
            var customer = $"{reservation.Customer.FirstName} {reservation.Customer.LastName}";
            var services = string.Join(", ", reservation.ReservationServices.Select(rs => rs.Service.Title));
            var startTime = reservation.StartTime.DateTimeToLocal(owner.TimeZone);
            var endTime = reservation.EndTime.DateTimeToLocal(owner.TimeZone);

            var user = await _ownerRepo.GetUser(ownerId);
            var userCulture = CultureHelper.MapCultureInfo(user.Language);

            var notificationTitle = _translations["NewReservationTitle." + user.Language];
            var notificationText = string.Format(_translations["OwnerReservationText." + user.Language], 
                startTime.ToString("ddd dd MMMM, H:mm", userCulture) + " - " + endTime.ToString("H:mm", userCulture)
                , services, customer);

            //Add data in to notification center
            var notification = new Notification()
            {
                Title = notificationTitle,
                Description = notificationText,
                NotificationDateUTC = DateTime.UtcNow,
                ReservationId = reservation.Id,
                NotificationType = NotificationType.NewReservationClient
            };

            await _ownerRepo.CreateNotification(ownerId, notification);

            //Add owner push notification messages to queue
            foreach (var cid in user.UserData)
            {
                await _notificationMessageRepo.CreateNotificationMessage(new NotificationMessage {

                    OwnerId = ownerId,
                    Channel = NotificationChannel.Push,
                    NotificationDateUTC = DateTime.UtcNow,
                    Recipient = cid.CID,
                    Platform = cid.Platform,
                    Title = notificationTitle,
                    Text = notificationText,
                    RedirectUrl = $"#/pages/reservation/edit?id={reservation.Id}"
                });
            }

            var chatId = reservation.Customer.ChatId;
            var language = reservation.Customer.ChatLanguage;
            if (chatId == null || chatId == 0)
            {
                var dbCustomer = await _ownerRepo.GetCustomer(ownerId, reservation.CustomerId.Value);
                chatId = dbCustomer.ChatId;
                language = dbCustomer.ChatLanguage;
            }
                

            //Add customer notifications if subscribed to telegram bot
            if (chatId != null && chatId != 0)
            {
                if (string.IsNullOrEmpty(language)) language = user.Language;
                var notificationCustomerText = string.Format(_translations["NewReservationCustomerText." + language],
                    owner.Title, services, startTime.ToString("ddd dd MMMM, H:mm", CultureHelper.MapCultureInfo(language)));
                await _notificationMessageRepo.CreateNotificationMessage(new NotificationMessage
                {
                    OwnerId = ownerId,
                    Channel = NotificationChannel.Telegram,
                    NotificationDateUTC = DateTime.UtcNow,
                    Recipient = chatId.ToString(),
                    Text = notificationCustomerText
                });
            }
        }

        public async Task ReservationUpdated(Guid ownerId, Reservation oldReservation, Reservation newReservation)
        {
            if (newReservation.StartTime < DateTime.UtcNow) return;

            var owner = await _ownerRepo.GetOwner(ownerId, true);

            var oldCustomer = $"{oldReservation.Customer.FirstName} {oldReservation.Customer.LastName}";
            var oldServices = string.Join(", ", oldReservation.ReservationServices.Select(rs => rs.Service.Title));
            var oldStartTime = oldReservation.StartTime.DateTimeToLocal(owner.TimeZone);
            var oldEndTime = oldReservation.EndTime.DateTimeToLocal(owner.TimeZone);

            var newCustomerDb = await _ownerRepo.GetCustomer(ownerId, newReservation.CustomerId.Value);
            var newCustomer = $"{newCustomerDb.FirstName} {newCustomerDb.LastName}";
            var newServices = string.Join(", ", newReservation.ReservationServices.Select(rs => _ownerRepo.GetService(ownerId, rs.ServiceId).Result.Title));
            var newStartTime = newReservation.StartTime.DateTimeToLocal(owner.TimeZone);
            var newEndTime = newReservation.EndTime.DateTimeToLocal(owner.TimeZone);

            var user = await _ownerRepo.GetUser(ownerId);
            var userCulture = CultureHelper.MapCultureInfo(user.Language);

            var notificationTitle = _translations["UpdateReservationTitle." + user.Language];

            var oldNotificationText = string.Format(_translations["OwnerReservationText." + user.Language],
                oldStartTime.ToString("ddd dd MMMM, H:mm", userCulture) + " - " + oldEndTime.ToString("H:mm", userCulture)
                , oldServices, oldCustomer); 
            var newNotificationText = string.Format(_translations["OwnerReservationText." + user.Language],
                newStartTime.ToString("ddd dd MMMM, H:mm", userCulture) + " - " + newEndTime.ToString("H:mm", userCulture)
                , newServices, newCustomer);

            //Add/Update data in notification center
            var oldNotification = await _ownerRepo.GetReservationNotification(ownerId, newReservation.Id);

            if (oldNotification != null)
            {
                oldNotification.Title = notificationTitle;
                oldNotification.Description = newNotificationText;
                oldNotification.Note = oldNotificationText;
                oldNotification.NotificationDateUTC = DateTime.UtcNow;
                oldNotification.ReservationId = newReservation.Id;
                oldNotification.NotificationType = NotificationType.ChangeReservationClient;

                await _ownerRepo.UpdateNotification(ownerId, oldNotification);
            }
            else
            {
                var notification = new Notification()
                {
                    Title = notificationTitle,
                    Description = newNotificationText,
                    Note = oldNotificationText,
                    NotificationDateUTC = DateTime.UtcNow,
                    ReservationId = newReservation.Id,
                    NotificationType = NotificationType.ChangeReservationClient
                };
                await _ownerRepo.CreateNotification(ownerId, notification);
            }

            //Add owner push notification messages to queue
            foreach (var cid in user.UserData)
            {
                await _notificationMessageRepo.CreateNotificationMessage(new NotificationMessage
                {

                    OwnerId = ownerId,
                    Channel = NotificationChannel.Push,
                    NotificationDateUTC = DateTime.UtcNow,
                    Recipient = cid.CID,
                    Platform = cid.Platform,
                    Title = notificationTitle,
                    Text = newNotificationText,
                    RedirectUrl = $"#/pages/reservation/edit?id={newReservation.Id}"
                });
            }

            //Add customer notifications if subscribed to telegram bot
            //if customer changed
            if (newReservation.CustomerId != oldReservation.CustomerId)
            {
                //New reservation message to new customer
                if (newCustomerDb.ChatId != null && newCustomerDb.ChatId.Value != 0)
                {
                    var newCustomerLanguage = newCustomerDb.ChatLanguage ?? user.Language;
                    var notificationCustomerText = string.Format(_translations["NewReservationCustomerText." + newCustomerLanguage],
                        owner.Title, newServices, newStartTime.ToString("ddd dd MMMM, H:mm", CultureHelper.MapCultureInfo(newCustomerLanguage)));
                    await _notificationMessageRepo.CreateNotificationMessage(new NotificationMessage
                    {
                        OwnerId = ownerId,
                        Channel = NotificationChannel.Telegram,
                        NotificationDateUTC = DateTime.UtcNow,
                        Recipient = newCustomerDb.ChatId.Value.ToString(),
                        Text = notificationCustomerText
                    });
                }

                var oldCustomerChatId = oldReservation.Customer.ChatId;
                var oldCusomterLanguage = oldReservation.Customer.ChatLanguage;
                if (oldCustomerChatId == null || oldCustomerChatId == 0) {
                    var oldCustomerDb = await _ownerRepo.GetCustomer(ownerId, oldReservation.CustomerId.Value);
                    oldCustomerChatId = oldCustomerDb.ChatId;
                    oldCusomterLanguage = oldCustomerDb.ChatLanguage;
                }
                //Reservation canceled to previous customer
                if (oldCustomerChatId != null && oldCustomerChatId != 0)
                {
                    if (string.IsNullOrEmpty(oldCusomterLanguage)) oldCusomterLanguage = user.Language;
                    var notificationCustomerText = string.Format(_translations["CancelReservationCustomerText." + oldCusomterLanguage],
                        owner.Title, oldStartTime.ToString("ddd dd MMMM, H:mm", CultureHelper.MapCultureInfo(oldCusomterLanguage)));
                    await _notificationMessageRepo.CreateNotificationMessage(new NotificationMessage
                    {
                        OwnerId = ownerId,
                        Channel = NotificationChannel.Telegram,
                        NotificationDateUTC = DateTime.UtcNow,
                        Recipient = oldCustomerChatId.Value.ToString(),
                        Text = notificationCustomerText
                    });
                }
            } 
            //any other changes
            else
            {
                if (newCustomerDb.ChatId != null && newCustomerDb.ChatId.Value != 0 && (newStartTime != oldStartTime || newServices != oldServices))
                {
                    var language = newCustomerDb.ChatLanguage ?? user.Language;
                    var notificationCustomerText = string.Format(_translations["UpdateReservationCustomerText." + language], owner.Title);
                    if (newServices != oldServices) notificationCustomerText += string.Format(_translations["UpdateReservationCustomerService." + language], oldServices, newServices);
                    if (newStartTime != oldStartTime) notificationCustomerText += string.Format(_translations["UpdateReservationCustomerDate." + language], 
                        oldStartTime.ToString("ddd dd MMMM, H:mm", CultureHelper.MapCultureInfo(language)), 
                        newStartTime.ToString("ddd dd MMMM, H:mm", CultureHelper.MapCultureInfo(language)));
                    await _notificationMessageRepo.CreateNotificationMessage(new NotificationMessage
                    {
                        OwnerId = ownerId,
                        Channel = NotificationChannel.Telegram,
                        NotificationDateUTC = DateTime.UtcNow,
                        Recipient = newCustomerDb.ChatId.Value.ToString(),
                        Text = notificationCustomerText
                    });
                }
            }

        }

        public async Task ReservationDeleted(Guid ownerId, Reservation reservation)
        {
            if (reservation.StartTime < DateTime.UtcNow) return;

            var owner = await _ownerRepo.GetOwner(ownerId, true);
            var startTime = reservation.StartTime.DateTimeToLocal(owner.TimeZone);
            var endTime = reservation.EndTime.DateTimeToLocal(owner.TimeZone);
            var customer = $"{reservation.Customer.FirstName} {reservation.Customer.LastName}";
            var services = string.Join(", ", reservation.ReservationServices.Select(rs => rs.Service.Title));
            

            var user = await _ownerRepo.GetUser(ownerId);
            var userCulture = CultureHelper.MapCultureInfo(user.Language);

            var notificationTitle = _translations["CancelReservationTitle." + user.Language];
            var notificationText = string.Format(_translations["OwnerReservationText." + user.Language],
                startTime.ToString("ddd dd MMMM, H:mm", userCulture) + " - " + endTime.ToString("H:mm", userCulture)
                , services, customer);

            //Add data in to notification center
            var notification = new Notification()
            {
                Title = notificationTitle,
                Description = notificationText,
                NotificationDateUTC = DateTime.UtcNow,
                NotificationType = NotificationType.CancelReservationClient,
                CustomerId = reservation.Customer.Id
            };

            await _ownerRepo.CreateNotification(ownerId, notification);

            //Add owner push notification messages to queue
            foreach (var cid in user.UserData)
            {
                await _notificationMessageRepo.CreateNotificationMessage(new NotificationMessage
                {

                    OwnerId = ownerId,
                    Channel = NotificationChannel.Push,
                    NotificationDateUTC = DateTime.UtcNow,
                    Recipient = cid.CID,
                    Platform = cid.Platform,
                    Title = notificationTitle,
                    Text = notificationText,
                    RedirectUrl = $"#/pages/customer/edit?id={reservation.CustomerId}&action=drmessage"
                });
            }

            var chatId = reservation.Customer.ChatId;
            var language = reservation.Customer.ChatLanguage;
            if (chatId == null || chatId == 0) {
                var customerDb = await _ownerRepo.GetCustomer(ownerId, reservation.CustomerId.Value);
                chatId = customerDb.ChatId;
                language = customerDb.ChatLanguage;
            }

            //Add customer notifications if subscribed to telegram bot
            if (chatId != null && chatId != 0)
            {
                if (string.IsNullOrEmpty(language)) language = user.Language;
                var notificationCustomerText = string.Format(_translations["CancelReservationCustomerText." + language],
                    owner.Title, startTime.ToString("ddd dd MMMM, H:mm", CultureHelper.MapCultureInfo(language)));
                await _notificationMessageRepo.CreateNotificationMessage(new NotificationMessage
                {
                    OwnerId = ownerId,
                    Channel = NotificationChannel.Telegram,
                    NotificationDateUTC = DateTime.UtcNow,
                    Recipient = chatId.ToString(),
                    Text = notificationCustomerText
                });
            }
        }

        public async Task CreateReservationReminder(Reservation reservation)
        {
            if (reservation.StartTime < DateTime.UtcNow) return;

            var owner = await _ownerRepo.GetOwner(reservation.OwnerId, true);

            var services = string.Join(", ", reservation.ReservationServices.Select(rs => rs.Service.Title));
            var startTime = reservation.StartTime.DateTimeToLocal(owner.TimeZone);
            var endTime = reservation.EndTime.DateTimeToLocal(owner.TimeZone);

            var user = await _ownerRepo.GetUser(reservation.OwnerId);
            var userCulture = CultureHelper.MapCultureInfo(user.Language);

            //Add customer notifications if subscribed to telegram bot
            if (reservation.Customer.ChatId != null && reservation.Customer.ChatId != 0)
            {
                var language = reservation.Customer.ChatLanguage ?? user.Language;
                var notificationCustomerText = string.Format(_translations["ReminderCustomerText." + language],
                    owner.Title, services, startTime.ToString("ddd dd MMMM, H:mm", CultureHelper.MapCultureInfo(language)));
                await _notificationMessageRepo.CreateNotificationMessage(new NotificationMessage
                {
                    OwnerId = reservation.OwnerId,
                    Channel = NotificationChannel.Telegram,
                    NotificationDateUTC = DateTime.UtcNow,
                    Recipient = reservation.Customer.ChatId.ToString(),
                    Text = notificationCustomerText
                });
            } 
            else
            {
                var customer = $"{reservation.Customer.FirstName} {reservation.Customer.LastName}";

                var notificationTitle = _translations["ReminderTitle." + user.Language];
                var notificationText = string.Format(_translations["ReminderMessage." + user.Language], customer ,
                    startTime.ToString("ddd dd MMMM, H:mm", userCulture) + " - " + endTime.ToString("H:mm", userCulture)
                    ,services);


                //Add data in to notification center
                var notification = new Notification()
                {
                    Title = notificationTitle,
                    Description = notificationText,
                    NotificationDateUTC = DateTime.UtcNow,
                    ReservationId = reservation.Id,
                    NotificationType = NotificationType.ReminderReservation
                };

                await _ownerRepo.CreateNotification(reservation.OwnerId, notification);

                //Add owner push notification messages to queue
                foreach (var cid in user.UserData)
                {
                    await _notificationMessageRepo.CreateNotificationMessage(new NotificationMessage
                    {

                        OwnerId = reservation.OwnerId,
                        Channel = NotificationChannel.Push,
                        NotificationDateUTC = DateTime.UtcNow,
                        Recipient = cid.CID,
                        Platform = cid.Platform,
                        Title = notificationTitle,
                        Text = notificationText,
                        RedirectUrl = $"#/pages/reservation/edit?id={reservation.Id}"
                    });
                }
            }
        }

        public async Task SendReservationConfirmation(Reservation reservation, string commandKey)
        {
            var owner = await _ownerRepo.GetOwner(reservation.OwnerId, true);

            var customer = $"{reservation.Customer.FirstName} {reservation.Customer.LastName}";
            var services = string.Join(", ", reservation.ReservationServices.Select(rs => rs.Service.Title));
            var startTime = reservation.StartTime.DateTimeToLocal(owner.TimeZone);
            var endTime = reservation.EndTime.DateTimeToLocal(owner.TimeZone);

            var user = await _ownerRepo.GetUser(reservation.OwnerId);
            var userCulture = CultureHelper.MapCultureInfo(user.Language);

            var chatId = reservation.Customer.ChatId;
            var language = reservation.Customer.ChatLanguage;
            if (chatId == null || chatId == 0)
            {
                var dbCustomer = await _ownerRepo.GetCustomer(reservation.OwnerId, reservation.CustomerId.Value);
                chatId = dbCustomer.ChatId;
                language = dbCustomer.ChatLanguage;
            }

            if (chatId != null && chatId != 0)
            {
                if (string.IsNullOrEmpty(language)) language = user.Language;
                var notificationCustomerText = string.Format(_translations["ReservationConfirmationCustomerText." + language],
                    owner.Title, services, startTime.ToString("ddd dd MMMM, H:mm", CultureHelper.MapCultureInfo(language)));
                await _notificationMessageRepo.CreateNotificationMessage(new NotificationMessage
                {
                    OwnerId = reservation.OwnerId,
                    Channel = NotificationChannel.TelegramInlineKeyboard,
                    NotificationDateUTC = DateTime.UtcNow,
                    Recipient = chatId.ToString(),
                    Text = notificationCustomerText,
                    RedirectUrl = JsonSerializer.Serialize(new List<InlineKeyboardButton>() { new InlineKeyboardButton(_translations["ConfirmButton." + language]) { CallbackData = commandKey} })
                });
            }
        }
    }
}
