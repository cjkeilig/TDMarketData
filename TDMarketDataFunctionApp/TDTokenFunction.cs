using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TDMarketData.Service;

namespace TDMarketDataFunctionApp
{
    public class TDTokenFunction
    {
        private readonly TDHttpClient _tdHttpClient;

        public TDTokenFunction(TDHttpClient tdHttpClient)
        {
            _tdHttpClient = tdHttpClient;
        }

        [FunctionName("TDTokenFunction")]
        public async Task EnsureAuthenticated(
            [TimerTrigger("0 */5 * * * *")] TimerInfo timerInfo,
            ILogger log)
        {
            log.LogInformation("Timer trigger function processed a request in EnsureAuthenticated");


            await _tdHttpClient.EnsureAuthenticated();

        }
    }
}
