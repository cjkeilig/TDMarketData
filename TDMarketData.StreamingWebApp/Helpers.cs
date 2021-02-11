using Microsoft.Extensions.Localization.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TDMarketData.StreamingWebApp
{
    public static class Helpers
    {
        public static string[] GetTop50OptionVolumeSymbols()
        {
            var top50FilePath = @"D:\MARKETDATA\OPTIONSTATS\top50OptionVolTickers.txt";
            var symbols = new List<string>();

            using (var streamReader = new StreamReader(top50FilePath))
            {
                while (!streamReader.EndOfStream)
                {
                    symbols.Add(streamReader.ReadLine());
                }
            }

            return symbols.ToArray();
        }
    }
}
