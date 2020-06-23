using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TDMarketData.Domain
{
    public class TDCandle
    {
        [JsonProperty("open")]
        public double Open { get; set; }

        [JsonProperty("high")]
        public double High { get; set; }

        [JsonProperty("low")]
        public double Low { get; set; }

        [JsonProperty("close")]
        public double Close { get; set; }

        [JsonProperty("volume")]
        public long Volume { get; set; }

        [JsonProperty("datetime")]
        public long Datetime { get; set; }
    }
}
