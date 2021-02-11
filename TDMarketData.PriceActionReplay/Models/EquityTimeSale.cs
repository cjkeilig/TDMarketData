using System;
using System.Collections.Generic;
using System.Text;

namespace TDMarketData.PriceActionReplay.Models
{
    public class EquityTimeSale
    {
        public string Symbol { get; set; }
        public long Timestamp { get; set; }
        public double Price { get; set; } 
        public double Quantity { get; set; }
        public double SharesForBid { get; set; }
    }
}
