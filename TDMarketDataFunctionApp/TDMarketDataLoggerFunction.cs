using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using TDMarketData.Domain;
using TDMarketData.Service;

namespace TDMarketDataFunctionApp
{
    public class TDMarketDataLoggerFunction
    {

        private readonly TDMarketDataService _tdMarketDataService;
        public TDMarketDataLoggerFunction(TDMarketDataService tdMarketDataService)
        {
            _tdMarketDataService = tdMarketDataService;
        }

        [FunctionName("TDMarketDataLoggerFunction")]
        public async Task Run(
            [TimerTrigger("*/5 * * * *")] TimerInfo timerInfo,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var quotes = await _tdMarketDataService.GetQuote("MSFT");

            var optionChain = await _tdMarketDataService.GetOptionChain(new TDOptionChainRequest { Symbol = "MSFT", StrikeCount = 4 });
            

        }
    }
}
