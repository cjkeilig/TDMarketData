using System;
using System.Collections.Generic;
using System.Text;

namespace TDMarketData.Service
{
    public class TDAuthToken
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }
        public long expires_in { get; set; }
        public long refresh_token_expires_in { get; set; }
        public string token_type { get; set; }
        public DateTime issued_date { get; set; }
        public DateTime refresh_issued_date { get; set; }
    }
}
