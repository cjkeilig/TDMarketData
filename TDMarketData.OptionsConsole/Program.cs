using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TDMarketData.Domain;
using TDMarketData.Domain.TableStorageDto;
using TDMarketData.Service;
using TDMarketData.Service.DataStorage;
using TDMarketData.Service.Extensions;

namespace TDMarketData.OptionsConsole
{
    class Program
    {
        static ServiceProvider ServiceProvider;
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
    .SetBasePath(Environment.CurrentDirectory)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();


            ServiceProvider = new ServiceCollection().RegisterServices(config).AddLogging()
                .BuildServiceProvider();

            Run().GetAwaiter().GetResult();
        }

        private static async Task Run()
        {
            try
            {
                Timer t = new Timer(GetOptionData, null, 0, 3600000);

                Console.WriteLine("Press any key to stop");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private static void GetOptionData(object state)
        {



            var now = DateTime.Now;

            if (now.Hour < 7 || now.Hour > 17)
            {
                return;
            }


            var optionCandles = new List<OptionCandle>();
            var storageService = new MarketDataLocalFileStorageService();

            using (var scope = ServiceProvider.CreateScope())
            {
                var tdMarketDataService = scope.ServiceProvider.GetService<TDMarketDataService>();

                using (var sr = new StreamReader("optionable_stocks_2000.txt"))
                {
                    while (!sr.EndOfStream)
                    {
                        var symbol = sr.ReadLine();

                        var optionChain = tdMarketDataService.GetOptionChain(new TDOptionChainRequest { Symbol = symbol }).GetAwaiter().GetResult();

                        Thread.Sleep(1000);

                        var options = new List<OptionExpDateMap>();

                        if (optionChain.PutExpDateMap != null && optionChain.PutExpDateMap.Count > 0)
                        {
                            options.AddRange(optionChain.PutExpDateMap.SelectMany(k => k.Value).SelectMany(kv => kv.Value));
                        }

                        if (optionChain.CallExpDateMap != null && optionChain.CallExpDateMap.Count > 0)
                        {
                            options.AddRange(optionChain.CallExpDateMap.SelectMany(k => k.Value).SelectMany(kv => kv.Value));
                        }

                        if (options.Count == 0)
                            continue;

                        var underlyingPrice = optionChain.UnderlyingPrice;

                        var optionsList = options.Select(o =>
                        {
                            return new OptionCandle
                            {
                                Symbol = o.Symbol,
                                Close = o.Last,
                                Volume = o.TotalVolume,
                                Datetime = string.Format("{0:yyyyMMddHHmm}", now),
                                Volatility = o.Volatility,
                                OpenInterest = o.OpenInterest,
                                PercentChange = o.PercentChange,
                                Bid = o.Bid,
                                Ask = o.Ask,
                                BidAskSize = o.BidAskSize,
                                UnderlyingPrice = underlyingPrice
                            };
                        }).ToList();

                        optionCandles.AddRange(optionsList);


                    }
                }

                storageService.SaveOptions(optionCandles).GetAwaiter().GetResult();



            }
        }
    }
}
