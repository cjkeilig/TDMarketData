using System;
using System.Collections.Generic;
using System.Text;

namespace TDMarketData.Domain
{
    public class TDOptionChainRequest
    {
        public string Symbol { get; set; }
        public int StrikeCount { get; set; }
    }
}
