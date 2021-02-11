using System;
using System.Collections.Generic;
using System.Text;
using TDMarketData.BackTesting.Data.Models;

namespace TDMarketData.PositionGenerator
{
    public class OptionTimeSaleStatistic
    {
        public OptionTimeSale OptionTimeSale { get; set; }
        public double ZScore { get; set; }
    }
}
