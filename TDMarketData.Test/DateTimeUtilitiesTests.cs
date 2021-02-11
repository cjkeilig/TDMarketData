using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TDMarketData.PositionGenerator;
using Xunit;

namespace TDMarketData.Test
{
    public class DateTimeUtilitiesTests
    {
        [Fact]
        public void GetMarketOpen_Works()
        {
            var startTime = DateTimeUtilities.GetMarketOpen(0, DateTime.UtcNow);

            var test = DateTimeUtilities.GetMarketOpen(-2, DateTime.UtcNow);
            var test2 = DateTimeUtilities.GetMarketOpen(0, new DateTime(2020, 11, 13, 22, 0, 0));
            var test5657 = DateTimeUtilities.GetMarketClose(0, new DateTime(2020, 10, 30, 22, 0, 0));

            var test3 = DateTimeUtilities.GetMarketOpen(-2, DateTime.UtcNow);
            var test4 = DateTimeUtilities.GetMarketClose(-2, DateTime.UtcNow);

            var st = DateTimeUtilities.GetMarketOpen(0, new DateTime(2020, 10, 5, 10, 0, 0));
            var st1 = DateTimeUtilities.GetMarketOpen(0, new DateTime(2020, 10, 2, 21, 0, 0));

            var test11 = DateTimeUtilities.GetMarketClose(0, new DateTime(2020, 10, 17, 21, 0, 0));


            var nextDayOpen = DateTimeUtilities.GetMarketOpen(-1, new DateTime(2020, 10, 29, 22, 0, 0, DateTimeKind.Utc));

        }


        [Fact]
        public void GoBackTest()
        {
            var startDate = new DateTime(2020, 10, 12, 18, 10, 0, DateTimeKind.Utc);
            var endDate = new DateTime(2020, 10, 28, 15, 0, 0, DateTimeKind.Utc);
            var test = DateTimeUtilities.GetTradingDaysBetween(startDate, endDate);

            for (int i = 0; i < 30; i++)
            {
                Debug.WriteLine($"{DateTimeUtilities.GetMarketOpen(i, DateTime.UtcNow)} - {DateTimeUtilities.GetMarketClose(i, DateTime.UtcNow)}");
            }


        }
    }
}
