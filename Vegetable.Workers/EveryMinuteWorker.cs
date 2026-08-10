using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Vegetable.Workers
{
    public class EveryMinuteWorker : BackgroundService
    {

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var delayed = false;
            DateTime lastRun = RoundToMinute(DateTime.UtcNow);

            while (!stoppingToken.IsCancellationRequested)
            {
                if (!delayed) {
                    lastRun = RoundToMinute(DateTime.UtcNow); 
                }

                await ExecuteEveryMinuteAsync(lastRun, stoppingToken);

                var lastRunComplete = DateTime.UtcNow;
                if (RoundToMinute(lastRun) == RoundToMinute(lastRunComplete))
                {
                    delayed = false;
                    await Task.Delay(TimeSpan.FromSeconds(60 - DateTime.UtcNow.Second), stoppingToken);
                }
                else
                {
                    delayed = true;
                    lastRun = lastRun.AddSeconds(60-lastRun.Second);
                }
            }
        }

        public virtual Task ExecuteEveryMinuteAsync(DateTime runningTime, CancellationToken cancellationToken) {
            return Task.FromResult(default(object));
        }

        public static DateTime RoundToMinute(DateTime time)
        {
            return new DateTime(time.Year, time.Month, time.Day,
                                time.Hour, time.Minute, 0, 0, time.Kind);
        }
    }
}
