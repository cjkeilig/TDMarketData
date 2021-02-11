using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TDMarketData.Domain.TableStorageDto;

namespace TDMarketData.Service.DataStorage
{
    public interface IMarketDataFileStorageService1
    {
        Task<JObject> GetSymbolVolumeSnapshot();
        Task SaveCandles(IEnumerable<Candle> candles);
        Task SaveOptions(IEnumerable<OptionCandle> options);
        Task SaveSymbolVolumeSnapshot(JObject symbolVolumeSnapshot);
    }
}