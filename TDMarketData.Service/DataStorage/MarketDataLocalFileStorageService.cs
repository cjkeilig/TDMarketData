using CsvHelper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TDMarketData.Domain.TableStorageDto;

namespace TDMarketData.Service.DataStorage
{
    public class MarketDataLocalFileStorageService : IMarketDataFileStorageService1
    {
        public Task<JObject> GetSymbolVolumeSnapshot()
        {
            throw new NotImplementedException();
        }

        public Task SaveCandles(IEnumerable<Candle> candles)
        {
            throw new NotImplementedException();
        }

        public Task SaveOptions(IEnumerable<OptionCandle> options)
        {
            var date = DateTime.Now;
            using (var writer = new StreamWriter(@"D:\MARKETDATA\OPTIONCANDLES\option_candle_" + date.ToString("MMddyymmss") + ".csv"))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(options);
                    writer.Flush();
                }
            }

            return Task.FromResult("");
        }

        public Task SaveSymbolVolumeSnapshot(JObject symbolVolumeSnapshot)
        {
            throw new NotImplementedException();
        }
    }
}
