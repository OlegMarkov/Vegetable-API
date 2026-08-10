using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vegetable.API.Services
{
    public static class ConfigurationService
    {
        [Obsolete]
        public static string GetEnvironmentValue(this IConfiguration configuration, string key)
        {
            var environment = configuration["Environment"];

            var envKey = $"{key}.{environment}";

            if (!string.IsNullOrEmpty(configuration[envKey]))
            {
                return configuration[envKey];
            }
            else
            {
                return configuration[key];
            }
        }

        [Obsolete]
        public static string GetEnvironmentConnectionString(this IConfiguration configuration, string key)
        {
            var environment = configuration["Environment"];

            var envKey = $"ConnectionStrings:{key}.{environment}";

            if (!string.IsNullOrEmpty(configuration[envKey]))
            {
                return configuration[envKey];
            }
            else
            {
                return configuration[$"ConnectionStrings:{key}"];
            }
        }

    }
}
