using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TDMarketData.Domain;
using TDMarketData.Domain.TableStorageDto;
using TDMarketData.Service;

namespace TDMarketDataFunctionApp
{
    public class TDQuoteDataLoggerFunction
    {

        private readonly TDMarketDataService _tdMarketDataService;
        private readonly MarketDataStorageService _marketDataStorageService;
        private readonly IMapper _mapper;
        private readonly TDApiSettings _tdApiSettings;

        public TDQuoteDataLoggerFunction(TDMarketDataService tdMarketDataService, MarketDataStorageService marketDataStorageService, IMapper mapper, TDApiSettings tdApiSettings )
        {
            _tdMarketDataService = tdMarketDataService;
            _marketDataStorageService = marketDataStorageService;
            _mapper = mapper;
            _tdApiSettings = tdApiSettings;

        }

        [FunctionName("TDQuoteDataLoggerFunction")]
        public async Task LogTDQuoteData(
            [TimerTrigger("0 0 8 * * 1-5")] TimerInfo timerInfo,
            //[TimerTrigger("*/1 * * * *")] TimerInfo timerInfo,
            ILogger log)
        {
            log.LogInformation("Timer trigger function processed a request in LogTDQuoteData");

            var symbols = _tdApiSettings.SymbolsToTrack;

            foreach (var symbol in symbols)
            {
                var candles = await _tdMarketDataService.GetPriceHistory(new TDPriceHistoryRequest { Frequency = "5", FrequencyType = "minute", Period = "1", PeriodType = "day", Symbol = symbol });
                var candleEntities = _mapper.Map<List<Candle>>(candles);

                candleEntities.ForEach(c => c.Symbol = symbol);

                await _marketDataStorageService.SaveCandles(candleEntities);
            }


        }
    }
}
