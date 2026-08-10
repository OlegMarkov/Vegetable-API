using Microsoft.Extensions.Caching.Memory;
using System;
using CDB.Captcha;

namespace Vegetable.API.Services
{
    public static class Captcha
    {

        public static string GenerateCaptchaCode(string key, int width, int height, IMemoryCache cache)
        {
            var ch = new CaptchaHelper(width,height);
            var code = ch.GetRandomEnDigitalText(4);
            cache.Set(key, code, TimeSpan.FromSeconds(120));
            var imgbyte = ch.GetEnDigitalCodeByte(code);

            return Convert.ToBase64String(imgbyte);
        }

        public static bool ValidateCaptchaCode(string key, string userInputCaptcha, IMemoryCache cache)
        {
            string originalCode;
            if (!cache.TryGetValue(key, out originalCode))
            {
                return false;
            }
            cache.Remove(key);
            return originalCode.Equals(userInputCaptcha, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
