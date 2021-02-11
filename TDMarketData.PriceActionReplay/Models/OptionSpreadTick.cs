using System;
using System.Collections.Generic;
using System.Text;
using TDMarketData.PositionGenerator;

namespace TDMarketData.PriceActionReplay.Models
{
    public class OptionSpreadTick
    {
        public long Timestamp { get; set;  }
        public string Symbol { get; set; }
        public double Bid { get; set; }
        public double Ask { get; set; }
        public int BidSize { get; set; }
        public int AskSize { get; set; }
        public double BidAskSpread
        {
            get
            {
                return Ask - Bid;
            }
        }

        public DateTime Time
        {
            get
            {
                return DateTimeUtilities.TimestampToDateTime(Timestamp).AddHours(-5);
            }
        }

    }
}
