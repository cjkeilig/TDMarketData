using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TDMarketData.Service.Utilities
{
    public static class TDUtilities
    {
        public static async Task WaitForAuth()
        {
            await Task.Delay(30000);
        }
    }
}
