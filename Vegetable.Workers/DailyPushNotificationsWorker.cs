using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Vegetable.Core.Database;
using Vegetable.Core.Services;
using Vegetable.Entities;

namespace Vegetable.Workers
{
    public class DailyPushNotificationsWorker : EveryMinuteWorker
    {
        private readonly ILogger<DailyPushNotificationsWorker> _logger;
        private readonly IOwnerRepo _ownerRepo;
        private readonly IPushService _pushService;
        private readonly IConfiguration _translations;
        private readonly INotificationMessageRepo _notificationMessageRepo;

        public DailyPushNotificationsWorker(ILogger<DailyPushNotificationsWorker> logger, IServiceProvider serviceProvider, IPushService pushService, IConfiguration configuration)
        {
            _logger = logger;
            _ownerRepo = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IOwnerRepo>();
            _pushService = pushService;
            _translations = configuration.GetSection("Translations:Notifications");
            _notificationMessageRepo = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<INotificationMessageRepo>();
        }

        public override async Task ExecuteEveryMinuteAsync(DateTime runningTime, CancellationToken cancellationToken)
        {
            _logger.LogInformation("------------------------> DailyPushNotificationsWorker running at: {time}", runningTime);
            var users = await _ownerRepo.GetUsersToPush(runningTime);
            foreach (var user in users)
            {
                if (user.Count > 0 && !string.IsNullOrEmpty(user.CID))
                {
                    //await _pushService.PushMessageToSingleAsync(user.CID, _translations["DailyTitle." + user.Language], string.Format(_translations["DailyContent." + user.Language], user.Count));
                    await _notificationMessageRepo.CreateNotificationMessage(new NotificationMessage
                    {
                        OwnerId = user.OwnerId,
                        Channel = NotificationChannel.Push,
                        NotificationDateUTC = DateTime.UtcNow,
                        Recipient = user.CID,
                        Platform = user.Platform,
                        Title = _translations["DailyTitle." + user.Language],
                        Text = string.Format(_translations["DailyContent." + user.Language], user.Count),
                        RedirectUrl = string.Empty
                    });
                }
            }
            _logger.LogInformation("------------------------> DailyPushNotificationsWorker done at: {time}", DateTime.UtcNow);
        }
    }
}