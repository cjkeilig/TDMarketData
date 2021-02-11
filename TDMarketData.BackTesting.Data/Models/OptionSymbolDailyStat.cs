using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TDMarketData.BackTesting.Data.Models
{
    public class OptionSymbolDailyStat
    {
        public OptionSymbolDailyStat()
        {

        }
        public int Id { get; set; }
        public string symbol { get; set; }
        public string totalVol { get; set; }
        public string putVol { get; set; }
        public string callVol { get; set; }
        public string iv { get; set; }
        public string vwap { get; set; }
        public string iv52High { get; set; }
        public string iv52Low { get; set; }
        public string percIV { get; set; }
        public string hv52High { get; set; }
        public string hv52Low { get; set; }
        public string percHV { get; set; }
        public string sizIdx { get; set; }
        public string callSizIdx { get; set; }
        public string putSizIdx { get; set; }
        public string volSizIdx { get; set; }
        public string stSizIdx { get; set; }
        public long CallNotionalValue { get; set; }
        public long PutNotionalValue { get; set; }
        public DateTime Date { get; set; }
        public OptionSymbolDailyStatDetail StatDetail { get; set; }
    }

    public class OptionSymbolDailyStatClassMap : ClassMap<OptionSymbolDailyStat> {


        public OptionSymbolDailyStatClassMap()
        {
            
            //AutoMap(CultureInfo.InvariantCulture);
            Map(o => o.CallNotionalValue).Ignore();
            Map(o => o.PutNotionalValue).Ignore();
            Map(o => o.Id).Ignore();
            Map(o => o.Date).Ignore();
            Map(o => o.StatDetail).Ignore();
            Map(o => o.symbol);
            Map(o => o.callSizIdx);
            Map(o => o.callVol);
            Map(o => o.hv52High);
            Map(o => o.hv52Low);
            Map(o => o.iv);
            Map(o => o.iv52High);
            Map(o => o.iv52Low);
            Map(o => o.percHV);
            Map(o => o.percIV);
            Map(o => o.putSizIdx);
            Map(o => o.putVol);
            Map(o => o.sizIdx);
            Map(o => o.stSizIdx);
            Map(o => o.totalVol);
            Map(o => o.volSizIdx);
            Map(o => o.vwap);

        }

    }
}
