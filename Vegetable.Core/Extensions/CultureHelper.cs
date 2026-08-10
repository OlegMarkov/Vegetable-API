using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Vegetable.Core.Extensions
{
    public static class CultureHelper
    {
        public static CultureInfo MapCultureInfo(string language)
        {

            var culture = "";
            switch (language)
            {
                case "ru":
                    culture = "ru-RU";
                    break;
                case "en":
                    culture = "en-US";
                    break;
                default:
                    culture = "ru-RU";
                    break;
            }

            return new CultureInfo(culture);
        }
    }
}
