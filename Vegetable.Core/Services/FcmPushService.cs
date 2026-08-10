using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace Vegetable.Core.Services
{
    /// <summary>
    /// Firebase Cloud Messaging implementation of <see cref="IPushService"/>.
    ///
    /// Added when the mobile app moved off HBuilderX to Capacitor. The DCloud
    /// runtime supplied a GeTui SDK that minted the client id
    /// <see cref="PushService"/> addresses; a Capacitor app has no such SDK, so
    /// the device id is now an FCM registration token instead.
    ///
    /// Both implementations stay registered — see
    /// <c>ServiceCollectionExtensions.UsePushApi</c> — and which one is live is
    /// decided by the <c>PushProvider</c> setting. That matters during the
    /// rollout: every device still running an HBuilderX build has a GeTui cid
    /// and nothing else can reach it.
    ///
    /// The payload contract is deliberately identical to the GeTui one. The app
    /// reads <c>{ title, body, url }</c> off the message data and navigates to
    /// <c>url</c>; keeping that shape is what lets the client code stay common
    /// between the two runtimes.
    /// </summary>
    public class FcmPushService : IPushService
    {
        private readonly FcmPushOptions _options;
        private readonly Lazy<FirebaseMessaging> _messaging;

        public FcmPushService(FcmPushOptions options)
        {
            _options = options;

            // Lazy rather than eager: constructing this reads a file off disk
            // and validates credentials. Doing that in the constructor would
            // take the whole API down at startup over a push misconfiguration.
            _messaging = new Lazy<FirebaseMessaging>(CreateMessaging);
        }

        private FirebaseMessaging CreateMessaging()
        {
            // FirebaseApp instances are process-global and throw if created
            // twice under the same name. The API and the Workers host are
            // separate processes, but a test host can load both.
            FirebaseApp app = FirebaseApp.GetInstance(_options.AppName)
                ?? FirebaseApp.Create(
                    new AppOptions
                    {
                        Credential = GoogleCredential.FromFile(_options.ServiceAccountJsonPath),
                        ProjectId = _options.ProjectId
                    },
                    _options.AppName);

            return FirebaseMessaging.GetMessaging(app);
        }

        public async Task<string> PushMessageToSingleAsync(string cid, string title, string content, string urlId = "", string platform = "ios")
        {
            if (string.IsNullOrEmpty(cid))
            {
                return "no registration token";
            }

            // Sent on every message, not just when there is a url: the client
            // reads title and body out of the data payload on the paths where
            // it re-raises a notification itself.
            var data = new Dictionary<string, string>
            {
                ["title"] = title ?? string.Empty,
                ["body"] = content ?? string.Empty,
                ["url"] = urlId ?? string.Empty
            };

            var message = new Message
            {
                Token = cid,
                // The visible notification. Sending this rather than a
                // data-only message is what lets the OS display it while the
                // app is backgrounded or killed, which is the case that matters.
                Notification = new Notification
                {
                    Title = title,
                    Body = content
                },
                Data = data,
                Android = new AndroidConfig
                {
                    Priority = Priority.High,
                    // GeTui expired undelivered pushes after 600s. Same here:
                    // a booking notification an hour late is noise.
                    TimeToLive = TimeSpan.FromSeconds(600),
                    Notification = new AndroidNotification
                    {
                        Title = title,
                        Body = content,
                        // Matches the meta-data in AndroidManifest.xml. Android
                        // draws this as a silhouette.
                        Icon = "ic_stat_notify",
                        Color = "#5980A6"
                    }
                },
                Apns = new ApnsConfig
                {
                    Aps = new Aps
                    {
                        Alert = new ApsAlert { Title = title, Body = content },
                        Sound = "default"
                    }
                }
            };

            // Note both platform blocks are always set, and the `platform`
            // argument is ignored. FCM applies whichever is relevant to the
            // token, which is more reliable than trusting the platform string
            // stored alongside the registration — those rows have been wrong
            // before, which is why the app re-registers when it finds one null.

            try
            {
                return await _messaging.Value.SendAsync(message);
            }
            catch (FirebaseMessagingException ex) when (ex.MessagingErrorCode == MessagingErrorCode.Unregistered
                                                        || ex.MessagingErrorCode == MessagingErrorCode.InvalidArgument)
            {
                // The app was uninstalled, or the token was rotated and this row
                // is stale. Not an outage — do not let it fail a whole batch.
                // The registration should be deleted; returning the reason puts
                // it in NotificationMessage.Result where it can be found.
                return $"stale token ({ex.MessagingErrorCode}): {ex.Message}";
            }
        }

        /// <summary>
        /// GeTui's broadcast-to-every-installed-app call. It has no live caller
        /// — the one reference in PingController is commented out — and FCM has
        /// no equivalent that does not require devices to have subscribed to a
        /// topic first. Implementing an untested broadcast would be worse than
        /// saying plainly that it is not here.
        /// </summary>
        public void PushMessageToApp()
        {
            throw new NotSupportedException(
                "Broadcast push is not implemented for FCM. It needs a topic that devices subscribe to; " +
                "no caller uses it today.");
        }
    }
}
