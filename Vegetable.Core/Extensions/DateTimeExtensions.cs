using System;
using TimeZoneConverter;

namespace Vegetable.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static TimeSpan TimeSpanToUtc(this TimeSpan timespan, string timeZone)
        {
            if (timespan == null || timeZone == null)
            {
                return timespan;
            }

            TimeZoneInfo timeZoneInfo = FindSystemTimeZoneByIdWindowsIana(timeZone);
            var offset = timeZoneInfo.GetUtcOffset(DateTime.UtcNow);
            timespan = timespan.Subtract(offset);
            if (timespan.Days > 0)
            {
                timespan = timespan.Subtract(new TimeSpan(1, 0, 0, 0));
            }
            if (timespan.Hours < 0)
            {
                timespan = timespan.Add(new TimeSpan(24, 0, 0));
            }
            return timespan;
        }

        public static DateTime DateTimeToUtc(this DateTime dateTime, string timeZone)
        {
            if (dateTime == null || timeZone == null)
            {
                return dateTime;
            }

            TimeZoneInfo timeZoneInfo = FindSystemTimeZoneByIdWindowsIana(timeZone);
            var offset = timeZoneInfo.GetUtcOffset(DateTime.UtcNow);
            return dateTime.Subtract(offset);
        }

        public static DateTime DateTimeToLocal(this DateTime dateTime, string timeZone)
        {
            if (dateTime == null || timeZone == null || dateTime.Kind == DateTimeKind.Local)
            {
                return dateTime;
            }
            TimeZoneInfo timeZoneInfo = FindSystemTimeZoneByIdWindowsIana(timeZone);
            var offset = timeZoneInfo.GetUtcOffset(DateTime.UtcNow);
            return dateTime.Add(offset);
        }

        public static DateTime StartDateUTC(this DateTime dateTime, string timeZone)
        {
            var dateTimeLocal = dateTime.Date + new TimeSpan(0, 0, 0);
            return dateTimeLocal.DateTimeToUtc(timeZone);
        }

        public static DateTime EndDateUTC(this DateTime dateTime, string timeZone)
        {
            var dateTimeLocal = dateTime.Date + new TimeSpan(23, 59, 59);
            return dateTimeLocal.DateTimeToUtc(timeZone);
        }

        public static TimeZoneInfo FindSystemTimeZoneByIdWindowsIana(string timeZone)
        {
            TimeZoneInfo convertedTimeZoneInfo;
            var timeZoneWindows = TZConvert.IanaToWindows(timeZone);
            try
            {
                convertedTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneWindows);
            }
            catch (TimeZoneNotFoundException exc)
            {
                convertedTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            }
            return convertedTimeZoneInfo;
        }

        public static DateTime TimeRoundUp(this DateTime dateTime, int step = 15)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0, dateTime.Kind).AddMinutes(dateTime.Minute % step == 0 ? 0 : step - dateTime.Minute % step);
        }

        public static DateTime TimeRoundDown(this DateTime dateTime, int step = 15)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0, dateTime.Kind).AddMinutes(-dateTime.Minute % step);

        }
    }
}
