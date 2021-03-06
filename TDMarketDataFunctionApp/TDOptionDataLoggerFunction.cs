using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using TDMarketData.Domain;
using TDMarketData.Domain.TableStorageDto;
using TDMarketData.Service;
using TDMarketData.Service.DataStorage;
using TDMarketData.Service.Utilities;

namespace TDMarketDataFunctionApp
{
    public class TDOptionDataLoggerFunction
    {

        private readonly TDMarketDataService _tdMarketDataService;
        private readonly MarketDataTableStorageService _marketDataStorageService;
        private readonly TDApiSettings _tdApiSettings;
        private readonly IMapper _mapper;
        private readonly MarketDataFileStorageService _marketDataFileStorageService;

        public TDOptionDataLoggerFunction(TDMarketDataService tdMarketDataService, TDApiSettings tdApiSettings, IMapper mapper, MarketDataFileStorageService marketDataFileStorageService, MarketDataTableStorageService marketDataStorageService)
        {
            _tdMarketDataService = tdMarketDataService;
            _tdApiSettings = tdApiSettings;
            _mapper = mapper;
            _marketDataStorageService = marketDataStorageService;
            _marketDataFileStorageService = marketDataFileStorageService;
        }

        [FunctionName("TDOptionDataLoggerFunction")]
        public async Task LogTDOptionData(
            [TimerTrigger("0 */5 12-22 * * *")] TimerInfo timerInfo,
            ILogger log)
        {
            log.LogInformation("Timer trigger function processed a request in LogTDOptionData");

            await TDUtilities.WaitForAuth();

            var symbols = _tdApiSettings.SymbolsToTrack;
            var symbolVolumeObject = await _marketDataFileStorageService.GetSymbolVolumeSnapshot();
            var symbolVolumeSnapshot = new JObject();
            foreach (var symbol in symbols)
            {
                var optionChain = await _tdMarketDataService.GetOptionChain(new TDOptionChainRequest { Symbol = symbol });
                var options = optionChain.PutExpDateMap.Concat(optionChain.CallExpDateMap).SelectMany(k => k.Value.SelectMany(kv => kv.Value));
                var optionEntities = _mapper.Map<List<Option>>(options);

                optionEntities.ForEach(o => symbolVolumeSnapshot[o.Symbol] = o.TotalVolume);

                //await _marketDataStorageService.SaveOptions(optionEntities);

                if (timerInfo.ScheduleStatus.Last != DateTime.MinValue)
                {
                    var candles = options.Where(o => symbolVolumeObject.TryGetValue(o.Symbol, StringComparison.OrdinalIgnoreCase, out JToken jToken)).Select(o =>
                    {
                        var volume = 0L;
                        var previousValue = symbolVolumeObject.GetValue(o.Symbol).Value<long>();

                        var currentValue = o.TotalVolume;

                        if (o.TotalVolume != 0)
                        {
                            volume = o.TotalVolume - previousValue;
                        }

                        return new OptionCandle
                        {
                            Symbol = o.Symbol,
                            Close = o.Last,
                            Volume = volume,
                            Datetime = string.Format("{0:yyyyMMddHHmm}", timerInfo.ScheduleStatus.Last),
                            Volatility = o.Volatility,
                            OpenInterest = o.OpenInterest,
                            PercentChange = o.PercentChange
                        };
                    }).ToList();

                    if (candles.Count > 0)
                        await _marketDataFileStorageService.SaveOptions(candles);

                }
            }


            await _marketDataFileStorageService.SaveSymbolVolumeSnapshot(symbolVolumeSnapshot);


        }
    }
}
