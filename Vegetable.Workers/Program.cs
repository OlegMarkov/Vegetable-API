using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Reflection;
using Telegram.Bot;
using Vegetable.Core.Database;
using Vegetable.Core.Extensions;
using Vegetable.Core.Services;

namespace Vegetable.Workers
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSystemd()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddDbContext<PostgreDbContext>(options =>
                        options.UseNpgsql(hostContext.Configuration["ConnectionStrings:Postgre"], b => b.MigrationsAssembly("Vegetable.Core")));
                    services.AddHttpClient("tgwebhook")
                        .AddTypedClient<ITelegramBotClient>(httpClient => new TelegramBotClient(hostContext.Configuration["BotConfiguration:BotToken"], httpClient));
                    services.AddHttpClient();
                    services.AddTransient<IOwnerRepo, OwnerRepo>();
                    services.AddTransient<INotificationMessageRepo, NotificationMessageRepo>();
                    services.AddTransient<INotificationsService, NotificationsService>();
                    services.UsePushApi(hostContext.Configuration);

                    services.AddHostedService<DailyPushNotificationsWorker>();
                    services.AddHostedService<ReservationReminderWorker>();
                    services.AddHostedService<NotificationMessagesSendWorker>();
                });
    }
}
