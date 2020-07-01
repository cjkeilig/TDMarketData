using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TDMarketData.Domain.TDStreaming
{
    public class TDStreamingRequest
    {
        [JsonProperty("service")]
        public string Service { get; set; }
        [JsonProperty("requestid")]
        public string RequestId { get; set; }
        [JsonProperty("command")]
        public string Command { get; set; }
        [JsonProperty("account")]
        public string Account { get; set; }
        [JsonProperty("source")]
        public string Source { get; set; }
        [JsonProperty("parameters")]
        public Dictionary<string, string> Parameters { get; set; }
    }
   
}
