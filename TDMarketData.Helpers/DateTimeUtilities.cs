using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Text;

namespace TDMarketData.PositionGenerator
{
    public static class DateTimeUtilities
    {
        public static Func<DateTime, bool> IsStandardTime = (DateTime date) => date <= new DateTime(2020, 11, 1);
        public static DateTime GetMarketOpen(int daysAgo, DateTime asOf)
        {
            var asOfYesterday = asOf.AddDays(-1);

            var isDayLightSavings = IsStandardTime(asOf);
            var closeHour = isDayLightSavings ? 21 : 22;

            var marketOpen = asOf.Hour >= closeHour ? GetMarketOpen(asOf.Year, asOf.Month, asOf.Day, asOf) : GetMarketOpen(asOfYesterday.Year, asOfYesterday.Month, asOfYesterday.Day, asOfYesterday);

            while ((int)marketOpen.DayOfWeek == 0 || (int)marketOpen.DayOfWeek == 6)
            {
                marketOpen = marketOpen.AddDays(-1);
            }


            var daysToSubtract = 0;

            if (daysAgo > 0)
            {
                var extraWeekEnd = (daysAgo % 5) >= (int)marketOpen.DayOfWeek ? 1 : 0;

                daysToSubtract = (((daysAgo / 5) + extraWeekEnd) * 2) + daysAgo;

            }
            else if (daysAgo < 0)
            {
                var extraDays = Math.Abs(daysAgo % 5);
                var daysLeftInWorkWeek = 5 - (int)marketOpen.DayOfWeek;
                var extraWeekEnd = (extraDays > daysLeftInWorkWeek) ? 1 : 0;

                daysToSubtract = (((daysAgo / 5) - extraWeekEnd) * 2) + daysAgo;
            }


            var date = marketOpen.AddDays(daysToSubtract * -1);

            var dstStopped = !isDayLightSavings && IsStandardTime(date);

            if (dstStopped)
            {
                date = date.AddHours(-1);
            }

            return date;

        }

        public static DateTime GetMarketClose(int daysAgo, DateTime asOf)
        {
            var asOfYesterday = asOf.AddDays(-1);

            var isDayLightSavings = IsStandardTime(asOf);
            var closeHour = isDayLightSavings ? 21 : 22;

            var marketClose = asOf.Hour >= closeHour ? GetMarketClose(asOf.Year, asOf.Month, asOf.Day, asOf) : GetMarketClose(asOfYesterday.Year, asOfYesterday.Month, asOfYesterday.Day, asOf);

            while ((int)marketClose.DayOfWeek == 0 || (int)marketClose.DayOfWeek == 6)
            {
                marketClose = marketClose.AddDays(-1);
            }

            var daysToSubtract = 0;

            if (daysAgo > 0)
            {
                var extraWeekEnd = (daysAgo % 5) >= (int)marketClose.DayOfWeek ? 1 : 0;

                daysToSubtract = (((daysAgo / 5) + extraWeekEnd) * 2) + daysAgo;

            }
            else if (daysAgo < 0)
            {

                var extraDays = Math.Abs(daysAgo % 5);
                var daysLeftInWorkWeek = 5 - (int)marketClose.DayOfWeek;
                var extraWeekEnd = (extraDays > daysLeftInWorkWeek) ? 1 : 0;

                daysToSubtract = (((daysAgo / 5) - extraWeekEnd) * 2) + daysAgo;
            }

            var date = marketClose.AddDays(daysToSubtract * -1);

            var dstStopped = !isDayLightSavings && IsStandardTime(date);

            if (dstStopped)
            {
                date = date.AddHours(-1);
            }

            return date;
        }

        public static DateTime OptionCandleTimestampToDateTime(long datetime)
        {
            var dateTimeStr = datetime.ToString();
            var year = int.Parse(dateTimeStr.Substring(0, 4));
            var month = int.Parse(dateTimeStr.Substring(4, 2));
            var day = int.Parse(dateTimeStr.Substring(6, 2));
            var hour = int.Parse(dateTimeStr.Substring(8, 2));
            var minute = int.Parse(dateTimeStr.Substring(10, 2));

            var date = new DateTime(year, month, day, hour, minute, 0);

            var hoursBehind = IsStandardTime(date) ? 5 : 6;

            var utcDate = date.AddHours(hoursBehind);

            return utcDate;
        }

        public static long DateTimeToOptionCandleTimestamp(DateTime datetime)
        {

            var result = string.Format("{0:yyyyMMddhhmm}", datetime);

            return long.Parse(result);
        }

        public static DateTime TimestampToDateTime(long timestamp)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var result = epoch.AddMilliseconds(timestamp);

            return result;
        }

        public static DateTime GetMarketOpen(int year, int month, int day, DateTime asOfDate)
        {

            var dayLightSavingsEnded = new DateTime(2020, 11, 1);

            if (asOfDate >= dayLightSavingsEnded)
            {
                return new DateTime(year, month, day, 14, 30, 0, DateTimeKind.Utc);
            }
            else
            {
                return new DateTime(year, month, day, 13, 30, 0, DateTimeKind.Utc);
            }
        }

        public static DateTime GetMarketClose(int year, int month, int day, DateTime asOfDate)
        {
            var dayLightSavingsEnded = new DateTime(2020, 11, 1);

            if (asOfDate >= dayLightSavingsEnded)
            {
                return new DateTime(year, month, day, 22, 0, 0, DateTimeKind.Utc);
            }
            else
            {
                return new DateTime(year, month, day, 21, 0, 0, DateTimeKind.Utc);
            }
        }

        public static int GetTradingDaysBetween(DateTime startDate, DateTime endDate)
        {
            var tempStartDate = GetMarketClose(0, startDate);
            var tempEndDate = GetMarketClose(0, endDate);

            var counter = 0;
            while (tempStartDate <= tempEndDate)
            {
                if ((int)tempStartDate.DayOfWeek != 6 && (int)tempStartDate.DayOfWeek != 0)
                {
                    counter++;
                }

                tempStartDate = tempStartDate.AddDays(1);
            }

            return counter;
            //var endHour = IsDayLightSavings(endDate) ? 21 : 22;
            //while (endDate.Hour < endHour)
            //{
            //    tempEndDate = tempEndDate.A
            //}

            //while (tempDate)

            //var dateTime = DateTime.UtcNow;
        }
    }
}
