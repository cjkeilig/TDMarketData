using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace TDMarketData.BackTesting.Data.Models
{
    public class OptionTimeSale
    {
        public int Id { get; set; }
        public int OptionContractId { get; set; }
        public OptionContract OptionContract { get; set; }
        public string Symbol { get; set; }
        public long Time { get; set; }
        public double Price { get; set; }
        public int Qty { get; set; }
        public double Bid { get; set; }
        public double Ask { get; set; }
        public double UnderlyingPrice { get; set; }
        public int TradeId { get; set; }
        public double NotionalValue
        {
            get
            {
                return Price * Qty * 100;
            }
        }
       // public IEnumerable<OptionTimeSale> TradeLegs { get; set; }

    }

    public sealed class OptionTimeSaleMap : ClassMap<OptionTimeSale>
    {
        public OptionTimeSaleMap()
        {
            Map(m => m.Symbol).Name("symbol");
            Map(m => m.Qty).Name("qty");
            Map(m => m.Time).ConvertUsing(row => long.Parse(row.GetField("time")));
            Map(m => m.Price).Name("price");
            Map(m => m.Bid).Name("bid");
            Map(m => m.Ask).Name("ask");
            Map(m => m.UnderlyingPrice).Name("underlyingPrice");

        }


    }
}
