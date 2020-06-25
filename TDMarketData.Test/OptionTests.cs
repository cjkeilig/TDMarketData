using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using TDMarketData.Domain.TableStorageDto;
using Xunit;

namespace TDMarketData.Test
{
    public class OptionTests
    {
        [Fact] 
        public void Options_Deserialize()
        {
            var option = new Option
            {
                Symbol = "SPY_1234",
                TotalVolume = 25
            };

            var jObject = new JObject();
            jObject[option.Symbol] = option.TotalVolume;

            var serialize = jObject.ToString();

            Assert.NotNull(serialize);
        }
    }
}
