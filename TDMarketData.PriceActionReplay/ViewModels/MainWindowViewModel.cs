using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup.Localizer;
using TDMarketData.PriceActionReplay.Models;

namespace TDMarketData.PriceActionReplay.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {

        public MainWindowViewModel()
        {
            EquityTimeSaleDict = new Dictionary<string, List<EquityTimeSale>>();
            OptionSpreadTickDict = new Dictionary<string, List<OptionSpreadTick>>();
        }

        private string _speed = "50";
        public string Speed
        {
            get => _speed;
            set
            {
                SetProperty<string>(ref _speed, value);
            }
        }

        private DateTime _dateTick = DateTime.Now;
        public DateTime DateTick
        {
            get => _dateTick;
            set
            {
                SetProperty<DateTime>(ref _dateTick, value);
            }
        }


        private string _selectedSymbol = null;
        public string SelectedSymbol
        {
            get => _selectedSymbol;
            set
            {
                SetProperty<string>(ref _selectedSymbol, value);

                if (AllOptionContracts == null)
                    return;

                SymbolOptionContracts = AllOptionContracts.Where(o => o.StartsWith(SelectedSymbol)).ToList();

                CurrentSymbolData = new SymbolData
                {
                    EquityTimeSale = EquityTimeSaleDict[SelectedSymbol],
                    OptionSpreadTickDict = OptionSpreadTickDict.Where(o => SymbolOptionContracts.Contains(o.Key)).ToDictionary(o => o.Key, o => o.Value)
                };
            }
        }

        private List<string> _symbols = null;
        public List<string> Symbols
        {
            get => _symbols;
            set
            {
                SetProperty<List<string>>(ref _symbols, value);
            }
        }

        private List<string> _allOptionContracts = null;
        public List<string> AllOptionContracts
        {
            get => _allOptionContracts;
            set
            {
                SetProperty<List<string>>(ref _allOptionContracts, value);
            }
        }

        private List<string> _symbolOptionContracts = null;
        public List<string> SymbolOptionContracts
        {
            get => _symbolOptionContracts;
            set
            {
                SetProperty<List<string>>(ref _symbolOptionContracts, value);
            }
        }


        public DateTime _selectedDate = DateTime.Now.Date;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (SetProperty<DateTime>(ref _selectedDate, value))
                {
                    RefreshSymbols();
                }
            }
        }


        public List<OptionSpreadTick> OptionSpreadTicks { get; set; }
        public List<EquityTimeSale> EquityTimeSales { get; set; }

        private Dictionary<string, List<OptionSpreadTick>> _optionSpreadTickDict = null;
        public Dictionary<string, List<OptionSpreadTick>> OptionSpreadTickDict
        {
            get => _optionSpreadTickDict;
            set
            {
                SetProperty<Dictionary<string, List<OptionSpreadTick>>>(ref _optionSpreadTickDict, value);
            }
        }


        public Dictionary<string, List<EquityTimeSale>> EquityTimeSaleDict { get; set; }
        public SymbolData _currentSymbolData = null;

        public SymbolData CurrentSymbolData
        {
            get => _currentSymbolData;
            set
            {
                SetProperty<SymbolData>(ref _currentSymbolData, value);
            }
        }



        private KeyValuePair<string, List<OptionSpreadTick>> _oSTKvp;

        public KeyValuePair<string, List<OptionSpreadTick>> OSTKvp 
        { 
            get => _oSTKvp;
            set
            {
                SetProperty<KeyValuePair<string, List<OptionSpreadTick>>>(ref _oSTKvp, value);
            } 
        
        }

        private DelegateCommand _commandStart = null;
        public DelegateCommand CommandStart =>
            _commandStart ?? (_commandStart = new DelegateCommand(async () => await CommandStartExecute()));

        private async Task CommandStartExecute()
        {
            var now = DateTime.Now;

            if (SelectedDate >= now)
                return;

            DateTick = SelectedDate;

            while (DateTick < now)
            {
                var msWait = 1000 / int.Parse(Speed);

                DateTick = DateTick.AddMinutes(1);

                await Task.Delay(1000);
            }
        }

        private async Task RefreshSymbols()
        {

            var dayFileFilter = string.Format("{0:yyMMdd}", SelectedDate);
            var optionBidAskFilePath = @"D:\MARKETDATA\OPTIONBIDASK";
            var equityTimeSaleFilePath = @"D:\MARKETDATA\EQUITYTIMESALE";
            var optionBidAskFile = Directory.GetFiles(optionBidAskFilePath, "wss_option_bid_ask_" + dayFileFilter + "*").First();
            var equityTimeSaleFile = Directory.GetFiles(equityTimeSaleFilePath, "wss_equity_timesale_" + dayFileFilter + "*").First();




            var rowsToScanForSymbols = 20;
            var underlyingSymbols = new List<string>();
            using (var streamReader = new StreamReader(equityTimeSaleFile))
            {
                for (var i = 0; i < rowsToScanForSymbols; i++)
                {
                    var line = await streamReader.ReadLineAsync();
                    var jEts = JObject.Parse(line);


                    var contentArray = jEts["content"] as JArray;


                    foreach (var token in contentArray)
                    {
                        var symbol = token.Value<string>("key");

                        if (!underlyingSymbols.Contains(symbol))
                        {
                            underlyingSymbols.Add(symbol);
                        }
                    }

                }

                streamReader.BaseStream.Position = 0;
                streamReader.DiscardBufferedData();
                var largestDiff = 0L;
                var smallestDiff = long.MaxValue;
                var prevTimestamp = 0L;

                var AllEquityTimeSales = new List<EquityTimeSale>();
                while (!streamReader.EndOfStream)
                {
                    var line = await streamReader.ReadLineAsync();
                    var jObj = JObject.Parse(line);

                    var timestamp = jObj.Value<long>("timestamp");

                    var diff = timestamp - prevTimestamp;
                    if (diff < smallestDiff)
                    {
                        smallestDiff = diff;
                    }

                    if (diff > largestDiff && diff < 60000)
                    {
                        largestDiff = diff;
                    }

                    prevTimestamp = timestamp;

                    var contentArray = jObj["content"] as JArray;


                    foreach (var token in contentArray)
                    {
                        var symbol = token.Value<string>("key");

                        var ets = new EquityTimeSale
                        {
                            Symbol = symbol,
                            Timestamp = token.Value<long>("1"),
                            Price = token.Value<double>("2"),
                            Quantity = token.Value<double>("3"),
                            SharesForBid = token.Value<double>("4")
                        };

                        AllEquityTimeSales.Add(ets);


                        if (EquityTimeSaleDict.ContainsKey(symbol))
                        {
                            var etsList = EquityTimeSaleDict[symbol];
                            etsList.Add(ets);
                        }
                        else
                        {
                            EquityTimeSaleDict.Add(symbol, new List<EquityTimeSale>() { ets });
                        }

                    }
                }

                EquityTimeSales = AllEquityTimeSales;


                // Debug.WriteLine($"Smallest time diff for equity time sale: {smallestDiff}\r\n, largest diff {largestDiff}");
            }

            Symbols = underlyingSymbols;

            var optionContracts = new List<string>();

            using (var streamReader = new StreamReader(optionBidAskFile))
            {
                for (var i = 0; i < rowsToScanForSymbols; i++)
                {
                    var line = await streamReader.ReadLineAsync();
                    var jEts = JObject.Parse(line);


                    var contentArray = jEts["content"] as JArray;


                    foreach (var token in contentArray)
                    {
                        var symbol = token.Value<string>("key");

                        if (!optionContracts.Contains(symbol))
                        {
                            optionContracts.Add(symbol);
                        }
                    }

                }


                streamReader.BaseStream.Position = 0;
                streamReader.DiscardBufferedData();

                var largestDiff = 0L;
                var smallestDiff = long.MaxValue;
                var prevTimestamp = 0L;


                var optionBAList = new List<OptionSpreadTick>();
                while (!streamReader.EndOfStream)
                {
                    var jObj = JObject.Parse(await streamReader.ReadLineAsync());



                    var timestamp = jObj.Value<long>("timestamp");

                    var diff = timestamp - prevTimestamp;
                    if (diff < smallestDiff)
                    {
                        smallestDiff = diff;
                    }

                    if (diff > largestDiff && diff < 60000)
                    {
                        largestDiff = diff;
                    }

                    prevTimestamp = timestamp;

                    var contentArray = jObj["content"] as JArray;

                    foreach (var token in contentArray)
                    {

                        var symbol = token.Value<string>("key");
                        var bid = token.Value<double>("2");
                        var ask = token.Value<double>("3");
                        var bidSize = token.Value<int>("20");
                        var askSize = token.Value<int>("21");
                        var ost = new OptionSpreadTick
                        {
                            Timestamp = timestamp,
                            Symbol = token.Value<string>("key"),
                            Bid = bid,
                            Ask = ask,
                            BidSize = bidSize,
                            AskSize = askSize
                        };

                        var prevOst = optionBAList.LastOrDefault(o => o.Symbol == symbol);

                        if (prevOst != null)
                        {
                            MergeChanges(prevOst, ost);
                        }

                        optionBAList.Add(ost);

                        if (OptionSpreadTickDict.ContainsKey(ost.Symbol))
                        {
                            var ostList = OptionSpreadTickDict[ost.Symbol];
                            ostList.Add(ost);
                        }
                        else
                        {
                            OptionSpreadTickDict.Add(ost.Symbol, new List<OptionSpreadTick> { ost });
                        }

                    }

                    OptionSpreadTicks = optionBAList;


                }

                Debug.WriteLine($"Smallest time diff for option bid ask: {smallestDiff}\r\n, largest diff {largestDiff}");

            }

            AllOptionContracts = optionContracts;

        }

        private void MergeChanges(OptionSpreadTick prevOst, OptionSpreadTick ost)
        {
            if (ost.Bid == 0D)
            {
                ost.Bid = prevOst.Bid;
            }

            if (ost.Ask == 0D)
            {
                ost.Ask = prevOst.Ask;
            }

            if (ost.BidSize == 0)
            {
                ost.BidSize = prevOst.BidSize;
            }

            if (ost.AskSize == 0)
            {
                ost.AskSize = prevOst.AskSize;
            }
        }
    }
}
