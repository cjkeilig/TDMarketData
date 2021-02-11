using CsvHelper.Configuration;
using System.Globalization;
using System.Text.Json.Serialization;

namespace TDMarketData.BackTesting.Data.Models
{
    public class OptionCandle
    {
        public int Id { get; set; }
        public int OptionContractId { get; set; }
        public string Symbol { get; set; }
        public double Close { get; set; }
        public long Volume { get; set; }
        public long Datetime { get; set; }
        public double Volatility { get; set; }
        public long OpenInterest { get; set; }
        public double PercentChange { get; set; }
        public string BidAskSize { get; set; }
        public double Bid { get; set; }
        public double Ask { get; set; }
        public double UnderlyingPrice { get; set; }
        [JsonIgnore]
        public OptionContract OptionContract { get; set; }
    }

    public sealed class OptionCandleMap : ClassMap<OptionCandle>
    {
        public OptionCandleMap()
        {
            Map(m => m.Symbol);
            Map(m => m.Volume);
            Map(m => m.Datetime).ConvertUsing(row => long.Parse(row.GetField("Datetime")));
            Map(m => m.OpenInterest);
            Map(m => m.BidAskSize);
            Map(o => o.Id).Ignore();
            Map(m => m.Close).ConvertUsing(row =>
            {
                var origValue = double.Parse(row.GetField("Close"));
                return double.IsNaN(origValue) ? 0.0D : origValue;

            });
            Map(m => m.Volatility).ConvertUsing(row =>
            {
                var origValue = double.Parse(row.GetField("Volatility"));
                return double.IsNaN(origValue) ? 0.0D : origValue;
            });
            Map(m => m.PercentChange).ConvertUsing(row =>
            {
                var origValue = double.Parse(row.GetField("PercentChange"));
                return double.IsNaN(origValue) ? 0.0D : origValue;

            });
            Map(m => m.Bid).ConvertUsing(row =>
            {
                var origValue = double.Parse(row.GetField("Bid"));
                return double.IsNaN(origValue) ? 0.0D : origValue;

            });
            Map(m => m.Ask).ConvertUsing(row =>
            {
                var origValue = double.Parse(row.GetField("Ask"));
                return double.IsNaN(origValue) ? 0.0D : origValue;

            });
            Map(m => m.UnderlyingPrice).ConvertUsing(row =>
            {
                var origValue = double.Parse(row.GetField("UnderlyingPrice"));
                return double.IsNaN(origValue) ? 0.0D : origValue;

            });

        }
    }
}