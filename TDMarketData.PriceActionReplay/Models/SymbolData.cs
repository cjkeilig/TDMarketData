using System;
using System.Collections.Generic;
using System.Text;

namespace TDMarketData.PriceActionReplay.Models
{
    public class SymbolData
    {
        public List<EquityTimeSale> EquityTimeSale { get; set; }
        public Dictionary<string, List<OptionSpreadTick>> OptionSpreadTickDict { get; set; }
    }
}
