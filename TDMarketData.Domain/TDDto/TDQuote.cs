using System;
using Newtonsoft.Json;

namespace TDMarketData.Domain
{
    public class TDQuote
    {

        [JsonProperty("assetType")]
        public string AssetType { get; set; }

        [JsonProperty("assetMainType")]
        public string AssetMainType { get; set; }

        [JsonProperty("cusip")]
        public string Cusip { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("bidPrice")]
        public double BidPrice { get; set; }

        [JsonProperty("bidSize")]
        public long BidSize { get; set; }

        [JsonProperty("bidId")]
        public string BidId { get; set; }

        [JsonProperty("askPrice")]
        public double AskPrice { get; set; }

        [JsonProperty("askSize")]
        public long AskSize { get; set; }

        [JsonProperty("askId")]
        public string AskId { get; set; }

        [JsonProperty("lastPrice")]
        public double LastPrice { get; set; }

        [JsonProperty("lastSize")]
        public long LastSize { get; set; }

        [JsonProperty("lastId")]
        public string LastId { get; set; }

        [JsonProperty("openPrice")]
        public double OpenPrice { get; set; }

        [JsonProperty("highPrice")]
        public double HighPrice { get; set; }

        [JsonProperty("lowPrice")]
        public double LowPrice { get; set; }

        [JsonProperty("bidTick")]
        public string BidTick { get; set; }

        [JsonProperty("closePrice")]
        public double ClosePrice { get; set; }

        [JsonProperty("netChange")]
        public double NetChange { get; set; }

        [JsonProperty("totalVolume")]
        public long TotalVolume { get; set; }

        [JsonProperty("quoteTimeInLong")]
        public long QuoteTimeInLong { get; set; }

        [JsonProperty("tradeTimeInLong")]
        public long TradeTimeInLong { get; set; }

        [JsonProperty("mark")]
        public double Mark { get; set; }

        [JsonProperty("exchange")]
        public string Exchange { get; set; }

        [JsonProperty("exchangeName")]
        public string ExchangeName { get; set; }

        [JsonProperty("marginable")]
        public bool Marginable { get; set; }

        [JsonProperty("shortable")]
        public bool Shortable { get; set; }

        [JsonProperty("volatility")]
        public double Volatility { get; set; }

        [JsonProperty("digits")]
        public long Digits { get; set; }

        [JsonProperty("52WkHigh")]
        public double The52WkHigh { get; set; }

        [JsonProperty("52WkLow")]
        public double The52WkLow { get; set; }

        [JsonProperty("nAV")]
        public long NAv { get; set; }

        [JsonProperty("peRatio")]
        public double PeRatio { get; set; }

        [JsonProperty("divAmount")]
        public double DivAmount { get; set; }

        [JsonProperty("divYield")]
        public double DivYield { get; set; }

        [JsonProperty("divDate")]
        public DateTimeOffset DivDate { get; set; }

        [JsonProperty("securityStatus")]
        public string SecurityStatus { get; set; }

        [JsonProperty("regularMarketLastPrice")]
        public double RegularMarketLastPrice { get; set; }

        [JsonProperty("regularMarketLastSize")]
        public long RegularMarketLastSize { get; set; }

        [JsonProperty("regularMarketNetChange")]
        public double RegularMarketNetChange { get; set; }

        [JsonProperty("regularMarketTradeTimeInLong")]
        public long RegularMarketTradeTimeInLong { get; set; }

        [JsonProperty("netPercentChangeInDouble")]
        public double NetPercentChangeInDouble { get; set; }

        [JsonProperty("markChangeInDouble")]
        public double MarkChangeInDouble { get; set; }

        [JsonProperty("markPercentChangeInDouble")]
        public double MarkPercentChangeInDouble { get; set; }

        [JsonProperty("regularMarketPercentChangeInDouble")]
        public double RegularMarketPercentChangeInDouble { get; set; }

        [JsonProperty("delayed")]
        public bool Delayed { get; set; }
    }
}
