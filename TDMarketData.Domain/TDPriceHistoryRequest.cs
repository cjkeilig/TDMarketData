using System;
using System.Collections.Generic;
using System.Text;

namespace TDMarketData.Domain
{
    public class TDPriceHistoryRequest
    {
        public string Period { get; set; }
        public string PeriodType { get; set; }
        public string Frequency { get; set; }
        public string FrequencyType { get; set; }
        public string Symbol { get; set; }
    }
}
