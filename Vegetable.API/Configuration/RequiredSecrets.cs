using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Vegetable.API.Configuration
{
    /// <summary>
    /// Checks at startup that the secrets the app cannot work without are
    /// actually present.
    ///
    /// This exists because the failure modes were bad. `Secret` is read as
    /// <c>Encoding.ASCII.GetBytes(_configuration["Secret"])</c> in both
    /// JwtMiddleware and AuthenticationService — with it unset that is
    /// GetBytes(null), a NullReferenceException on the first authenticated
    /// request rather than at boot, and nothing in the message says "secret".
    /// A missing database password surfaces as a connection error somewhere in
    /// the first query. Neither points at the cause.
    ///
    /// Since the values moved out of appsettings.json and into the environment,
    /// "someone forgot to set it on this host" became the likely mistake, so it
    /// is worth failing loudly and immediately.
    /// </summary>
    public static class RequiredSecrets
    {
        /// <summary>
        /// Configuration keys that must be non-empty for the API to serve
        /// traffic at all. Deliberately short: things that are merely
        /// feature-breaking (push, storage, Telegram) are left out, because
        /// refusing to boot over a broken notification channel is worse than
        /// running without it.
        /// </summary>
        private static readonly (string Key, string Why)[] Required =
        {
            ("Secret", "signs and validates every auth token"),
            ("ConnectionStrings:Postgre", "the database")
        };

        /// <summary>
        /// Keys whose absence disables a feature rather than the app. Reported
        /// so an operator sees it once at boot instead of discovering it from a
        /// user.
        /// </summary>
        private static readonly (string Key, string What)[] Optional =
        {
            ("GreenSms:Pass", "SMS verification calls; login will not work"),
            ("Auth0:UserClientSecret", "Auth0 user management"),
            ("YandexStorage:SecretKey", "image upload"),
            ("BotConfiguration:BotToken", "Telegram notifications"),
            ("Google:Secret", "reCAPTCHA validation on public booking")
        };

        /// <summary>
        /// Throws if a required secret is missing. Returns the list of missing
        /// optional ones so the caller can log them.
        /// </summary>
        public static IReadOnlyList<string> Validate(IConfiguration configuration)
        {
            var missing = Required
                .Where(entry => string.IsNullOrWhiteSpace(configuration[entry.Key]))
                .ToList();

            if (missing.Count > 0)
            {
                string detail = string.Join(
                    Environment.NewLine,
                    missing.Select(entry => $"  {entry.Key}  ({entry.Why})"));

                throw new InvalidOperationException(
                    "Required configuration is missing:" + Environment.NewLine +
                    detail + Environment.NewLine + Environment.NewLine +
                    // ASCII only: this lands in a Windows console, where the
                    // active code page mangles anything else.
                    "These are no longer stored in appsettings.json. Supply them as environment " +
                    "variables, using a double underscore for the section separator: " +
                    "ConnectionStrings__Postgre, Auth0__UserClientSecret, and so on. " +
                    "See SECRETS.md.");
            }

            return Optional
                .Where(entry => string.IsNullOrWhiteSpace(configuration[entry.Key]))
                .Select(entry => $"{entry.Key} is not set: {entry.What}")
                .ToList();
        }
    }
}
