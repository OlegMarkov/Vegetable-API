using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Vegetable.Core.Database;
using Vegetable.Core.Services;

namespace Vegetable.Workers
{
    public class ReservationReminderWorker : EveryMinuteWorker
    {
        private readonly ILogger<ReservationReminderWorker> _logger;
        private readonly IOwnerRepo _ownerRepo;
        private readonly INotificationsService _notificationService;

        public ReservationReminderWorker(ILogger<ReservationReminderWorker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _ownerRepo = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IOwnerRepo>();
            _notificationService = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<INotificationsService>();
        }

        public override async Task ExecuteEveryMinuteAsync(DateTime runningTime, CancellationToken cancellationToken)
        {
            _logger.LogInformation("------------------------> ReservationReminderWorker running at: {time}", runningTime);
            var reservations = await _ownerRepo.GetReservationsForSendingReminder(runningTime);
            foreach (var reservation in reservations)
            {
                await _notificationService.CreateReservationReminder(reservation);
            }
            _logger.LogInformation("------------------------> ReservationReminderWorker done at: {time}", DateTime.UtcNow);
        }
    }
}