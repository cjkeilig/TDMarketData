using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TDMarketData.Service;
using TDMarketData.Service.DataStorage;
using TDMarketData.Service.Utilities;

namespace TDMarketDataFunctionApp
{
    public class TDResetVolumeSnapshotFunction
    {
        private readonly MarketDataFileStorageService _marketDataStorageService;

        public TDResetVolumeSnapshotFunction(MarketDataFileStorageService marketDataStorageService)
        {
            _marketDataStorageService = marketDataStorageService;
        }

    [FunctionName("TDResetVolumeSnapshotFunction")]
        public async Task ResetVolumeSnapshotFunction(
            [TimerTrigger("0 0 10 * * *")] TimerInfo timerInfo,
            ILogger log)
        {
            log.LogInformation("Timer trigger function processed a request in EnsureAuthenticated");

            await TDUtilities.WaitForAuth();

            await _marketDataStorageService.SaveSymbolVolumeSnapshot(new JObject());

        }
    }
}
