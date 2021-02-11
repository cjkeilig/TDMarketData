using System;
using System.Collections.Generic;
using System.Text;

namespace TDMarketData.BackTesting.Data.Models
{
    public class OptionSymbolDailyStatDetail
    {
        public int Id { get; set; }
        public int OptionSymbolDailyStatId { get; set; }
        public OptionSymbolDailyStat DailyStat { get; set;}
        public double PutCallRatio5DayMvgAvg { get; set; }
        public double PutCallRatio20DayMvgAvg { get; set; }
        public double PutCallRatio30DayMvgAvg { get; set; }
        public double PutCallRatio180DayMvgAvg { get; set; }
        public double Volume5DayMvgAvg { get; set; }
        public double Volume20DayMvgAvg { get; set; }
        public double Volume30DayMvgAvg { get; set; }
        public double Volume180DayMvgAvg { get; set; }
        public double Volatility5DayMvgAvg { get; set; }
        public double Volatility20DayMvgAvg { get; set; }
        public double Volatility30DayMvgAvg { get; set; }
        public double Volatility180DayMvgAvg { get; set; }
        public double NotionalValue5DayMvgAvg { get; set; }
        public double NotionalValue20DayMvgAvg { get; set; }
        public double NotionalValue30DayMvgAvg { get; set; }
        public double NotionalValue180DayMvgAvg { get; set; }
        // Percent of trades that make up 10 percent of days notional value ordered by notional value descending
        public double NotionalValueXLPerc10 { get; set; }
        public double NotionalValueXLPerc20 { get; set; }
        public double NotionalValueXLPerc30 { get; set; }
        public double NotionalValueXLPerc40 { get; set; }
        public double NotionalValueXLPerc50 { get; set; }
        public double NotionalValueXLPerc60 { get; set; }
        public double NotionalValueXLPerc70 { get; set; }
        public double NotionalValueXLPerc80 { get; set; }
        public double NotionalValueXLPerc90 { get; set; }

    }
}
