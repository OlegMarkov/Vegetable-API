using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Vegetable.API.Services
{
    public class ConfigureWebhook : IHostedService
    {
        private readonly ILogger<ConfigureWebhook> _logger;
        private readonly IServiceProvider _services;
        private readonly BotConfiguration _botConfig;

        public ConfigureWebhook(ILogger<ConfigureWebhook> logger,
                                IServiceProvider serviceProvider,
                                IConfiguration configuration)
        {
            _logger = logger;
            _services = serviceProvider;
            _botConfig = configuration.GetSection("BotConfiguration").Get<BotConfiguration>();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _services.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

            // Configure custom endpoint per Telegram API recommendations:
            // https://core.telegram.org/bots/api#setwebhook
            // If you'd like to make sure that the Webhook request comes from Telegram, we recommend
            // using a secret path in the URL, e.g. https://www.example.com/<token>.
            // Since nobody else knows your bot's token, you can be pretty sure it's us.
            var webhookAddress = @$"{_botConfig.HostAddress}/bot/{_botConfig.BotToken}";
            _logger.LogInformation("Setting webhook: {webhookAddress}", webhookAddress);
            if (string.IsNullOrEmpty(_botConfig.CertPath))
            {
                await botClient.SetWebhookAsync(
                    url: webhookAddress,
                    allowedUpdates: Array.Empty<UpdateType>(),
                    cancellationToken: cancellationToken);
            }
            else
            {
                using var fileStream = new FileStream(_botConfig.CertPath, FileMode.Open);
                await botClient.SetWebhookAsync(
                    url: webhookAddress,
                    certificate: new Telegram.Bot.Types.InputFiles.InputFileStream(fileStream),
                    allowedUpdates: Array.Empty<UpdateType>(),
                    cancellationToken: cancellationToken);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            using var scope = _services.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

            // Remove webhook upon app shutdown
            _logger.LogInformation("Removing webhook");
            await botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
        }
    }

    public class BotConfiguration
    {
        public bool SetWebhook { get; set; }
        public string BotToken { get; set; }
        public string BotName { get; set; }
        public string HostAddress { get; set; }
        public string CertPath { get; set; }
    }
}
