using System;
using System.Collections.Generic;
using System.Text;

namespace TDMarketData.BackTesting.Data.Models
{
    public class OptionContract
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public string UnderlyingSymbol { get; set; }
        public double Strike { get; set; }
        public DateTime Expiration { get; set; }
        public char CallPut { get; set; }
        public ICollection<OptionCandle> OptionCandles { get; set; }
    }
}
