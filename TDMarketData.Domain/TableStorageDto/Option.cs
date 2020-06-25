using System;
using System.Collections.Generic;
using System.Text;

namespace TDMarketData.Domain.TableStorageDto
{
    public class Option
    {
        public string Symbol { get; set; }
        public string PutCall { get; set; }

        public string Description { get; set; }

        public double HighPrice { get; set; }

        public double LowPrice { get; set; }

        public long OpenPrice { get; set; }

        public double ClosePrice { get; set; }

        public long TotalVolume { get; set; }

        public double NetChange { get; set; }
        public double Volatility { get; set; }

        public long OpenInterest { get; set; }

        public double StrikePrice { get; set; }

        public long ExpirationDate { get; set; } 

        public double PercentChange { get; set; }

    }
}
