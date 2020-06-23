using System;
using System.Collections.Generic;
using System.Text;

namespace TDMarketData.Service
{
    public class TDApiSettings
    {
        public string BaseAddress { get; set; }
        public string TokenUri { get; set; }
        public string QuoteUri { get; set; }
        public string OptionChainUri { get; set; }
        public string ConsumerKey { get; set; }
        public string AuthUrl { get; set; }
        public string AuthCode { get; set; }
        public string LastAccessToken { get; set; }
        public string LastRefreshToken { get; set; }
        public long LastExpires { get; set; }
        public int RefreshTokenBufferPeriodMinutes { get; set; }

    }
}
