using GeTuiPushApiV2.ServerSDK.Core;
using GeTuiPushApiV2.ServerSDK.Core.MemoryCache;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vegetable.Core.Services;

namespace Vegetable.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the push stack and picks which provider is live.
        ///
        /// Set <c>PushProvider</c> to <c>Fcm</c> to send through Firebase;
        /// anything else (including it being absent) keeps GeTui, which is what
        /// the HBuilderX builds of the app registered with.
        ///
        /// Both sets of infrastructure are registered either way. During the
        /// rollout the two coexist: a device on an HBuilderX build has a GeTui
        /// cid and only GeTui can reach it, while a Capacitor build has an FCM
        /// token and only Firebase can. Switching this setting decides which
        /// population gets notified, so it should not flip until the Capacitor
        /// build is the one people are running.
        /// </summary>
        public static void UsePushApi(this IServiceCollection services, IConfiguration configuration)
        {
            AddGeTuiInfrastructure(services);
            AddGeTuiPushOptions(services, configuration);

            string provider = configuration["PushProvider"]?.Trim().ToLowerInvariant();

            if (provider == "fcm")
            {
                AddFcmPushOptions(services, configuration);
                services.AddSingleton<IPushService, FcmPushService>();
            }
            else
            {
                services.AddSingleton<IPushService, PushService>();
            }
        }

        private static void AddGeTuiInfrastructure(IServiceCollection services)
        {
            services.AddGeTuiPushApiV2();
            services.AddGeTuiPushService();
            services.AddMemoryCache();
            services.AddMemoryCacheStorage();
        }

        /// <summary>
        /// Bound through <see cref="IConfiguration"/> rather than by reading
        /// appsettings.json off disk, which is what this used to do.
        ///
        /// That mattered more than it looks: File.ReadAllText sees only the file,
        /// so the AppKey and MasterSecret could not be supplied by an environment
        /// variable, a user-secrets store or anything else in the configuration
        /// chain — they had to be committed to be usable. Going through
        /// IConfiguration is what lets them live outside the repo.
        /// </summary>
        public static void AddGeTuiPushOptions(this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.GetSection("GeTuiPushOptions").Get<GeTuiPushOptions>();
            if (options != null)
            {
                services.AddSingleton(options);
            }
        }

        public static void AddFcmPushOptions(this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.GetSection("FcmPushOptions").Get<FcmPushOptions>() ?? new FcmPushOptions();
            services.AddSingleton(options);
        }
    }
}
