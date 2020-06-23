using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using TDMarketData.Domain;
using TDMarketData.Service;

namespace TDMarketDataFunctionApp
{
    public class TDOptionDataLoggerFunction
    {

        private readonly TDMarketDataService _tdMarketDataService;
        public TDOptionDataLoggerFunction(TDMarketDataService tdMarketDataService)
        {
            _tdMarketDataService = tdMarketDataService;
        }

        //[FunctionName("TDOptionDataLoggerFunction")]
        //public async Task LogTDOptionData(
        //    [TimerTrigger("*/5 * * * *")] TimerInfo timerInfo,
        //    ILogger log)
        //{
        //    log.LogInformation("Timer trigger function processed a request in LogTDOptionData");

        //    var symbol = "MSFT";

        //    var optionChain = await _tdMarketDataService.GetOptionChain(new TDOptionChainRequest { Symbol = symbol, StrikeCount = 4 });
            

        //}
    }
}
