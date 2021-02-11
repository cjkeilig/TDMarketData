using CsvHelper;
using MathNet.Numerics.Statistics;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Threading;
using System.Threading.Tasks;
using TDMarketData.BackTesting.Data;
using TDMarketData.BackTesting.Data.Models;

namespace TDMarketData.PositionGenerator
{

    class Program
    {
        private static List<long> CandleDates;

        static void Main(string[] args)
        {
            //var period = 1000 * 60 * 60 * 24;


            //var today = DateTime.Now;
            //DateTime runAt;
            //if (today.Hour >= 9)
            //{
            //    var tomorrow = today;   //.AddDays(1);
            //    runAt = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 10, 40, 0);
            //}
            //else
            //{
            //    runAt = new DateTime(today.Year, today.Month, today.Day, 9, 0, 0);
            //}

            //var dueTime = (int)runAt.Subtract(today).TotalMilliseconds;
            //var positionGeneratorTime = new Timer((object state) => GenerateTop100ZScorePositions(0), null, dueTime, period);
         // GenerateCsv().GetAwaiter().GetResult();

           GeneratePosSummary().GetAwaiter().GetResult();
        }


        private async static Task GeneratePosSummary()
        {

            var filePath = @"D:\MARKETDATA\LARGEORDEROUTPUT";
            var fileFilter = "largorder_output_notv500k_qty500201203*";

            var files = Directory.GetFiles(filePath, fileFilter);

            var openedSingleBuys = new List<LargeTradeSummary>();

            foreach (var file in files)
            {

                using (var streamReader = new StreamReader(file))
                {
                    using (var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                    {

                        var records = csv.GetRecords<LargeTradeSummary>();

                        var openedSingleBuy = records.Where(r => r.SingleLeg && r.OpenedClosed == "Opened" && (r.BuySellStatus == "STRONG_BUY" || r.BuySellStatus == "LIKELY_BUY")).ToList();

                        if (openedSingleBuy.Count > 0)
                        {
                            openedSingleBuys.AddRange(openedSingleBuy);
                        }

                    }
                }
            }

            var context = new BackTestContext();

            var osbAnalysis = openedSingleBuys.Select(o =>
            {
                try
                {
                    var cstTime = o.Time.AddHours(-5);
                    var nextDay = cstTime.AddDays(1);
                    var nextDayStart = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, 0, 0, 0);

                    var dateTime = long.Parse(string.Format("{0:yyyyMMdd}0000", nextDayStart));
                    var candles = context.OptionCandles.Where(oc => oc.Datetime >= dateTime && oc.OptionContractId == o.OptionContractId).ToList();

                    var maxBid = candles.Select(c => c.Bid).Max();
                    var minBid = candles.Select(c => c.Bid).Min();

                    var maxBidCandle = candles.First(c => c.Bid == maxBid);
                    var minBidCandle = candles.First(c => c.Bid == minBid);

                    var maxNotionalValue = maxBid * o.Qty * 100;
                    var minNotionalValue = minBid * o.Qty * 100;

                    var maxNotValueDateTime = DateTimeUtilities.OptionCandleTimestampToDateTime(maxBidCandle.Datetime);
                    var maxDrawdownDateTime = DateTimeUtilities.OptionCandleTimestampToDateTime(minBidCandle.Datetime);

                    var daysTillMaxPL = DateTimeUtilities.GetTradingDaysBetween(o.Time, maxNotValueDateTime);
                    var daysTillMaxDrawDown = DateTimeUtilities.GetTradingDaysBetween(o.Time, maxDrawdownDateTime);

                    var maxPL = maxNotionalValue - o.NotionalValue;
                    var maxDrawdown = minNotionalValue - o.NotionalValue;

                    var maxPLPercent = ((maxNotionalValue - o.NotionalValue) / o.NotionalValue) * 100;
                    var maxDrawdownPercent = ((minNotionalValue - o.NotionalValue) / o.NotionalValue) * 100;

                    var candlesUpToMaxPL = candles.Where(c => c.Datetime <= maxBidCandle.Datetime);
                    var minBidUpToMaxPL = candlesUpToMaxPL.Select(c => c.Bid).Min();
                    var minBidUpToMaxPLCandle = candlesUpToMaxPL.First(c => c.Bid == minBidUpToMaxPL);
                    var minNotionalValueUpToMaxPL = minBidUpToMaxPL * o.Qty * 100;
                    var maxDrawDownUpToMaxPLDateTime = DateTimeUtilities.OptionCandleTimestampToDateTime(minBidUpToMaxPLCandle.Datetime);
                    var daysTillMaxDrawDownUpToMaxPL = DateTimeUtilities.GetTradingDaysBetween(o.Time, maxDrawDownUpToMaxPLDateTime);
                    var maxDrawdownUpToMaxPL = minNotionalValueUpToMaxPL - o.NotionalValue;
                    var maxDrawdownPercentUpToMaxPL = ((minNotionalValueUpToMaxPL - o.NotionalValue) / o.NotionalValue) * 100;

                    return new LargeOrderAnalysis
                    {

                        Id = o.Id,
                        Symbol = o.Symbol,
                        Expiration = o.Expiration,
                        Timestamp = o.Timestamp,
                        OptionContractId = o.OptionContractId,
                        Price = o.Price,
                        Bid = o.Bid,
                        Ask = o.Ask,
                        Qty = o.Qty,
                        NotionalValue = o.NotionalValue,
                        OpenInterestChange = o.OpenInterestChange,
                        OpenedClosed = o.OpenedClosed,
                        OptionTimeSaleId = o.OptionTimeSaleId,
                        LargeTradeSummaryParentTradeId = o.LargeTradeSummaryParentTradeId,
                        IsParentTrade = o.IsParentTrade,
                        SingleLeg = o.SingleLeg,
                        DayVolume = o.DayVolume,
                        UnderlyingPrice = o.UnderlyingPrice,
                        UnderlyingPriceClose = o.UnderlyingPriceClose,
                        UnderlyingPriceNextDayOpen = o.UnderlyingPriceNextDayOpen,
                        MaxPL = maxPL,
                        MaxPLPercent = maxPLPercent,
                        MaxNotValueDateTime = maxNotValueDateTime,
                        DaysTillMaxPL = daysTillMaxPL,
                        MaxDrawdown = maxDrawdown,
                        MaxDrowdownPercent = maxDrawdownPercent,
                        MaxDrawDownDateTime = maxDrawdownDateTime,
                        DaysTillMaxDrawdown = daysTillMaxDrawDown,
                        MinBid = minBid,
                        MaxBid = maxBid,
                        MaxDrawdownCandleId = minBidCandle.Id,
                        MaxPLCandleId = maxBidCandle.Id,
                        MaxDrawdownBeforeMaxPL = maxDrawdownUpToMaxPL,
                        MaxDrowdownPercentBeforeMaxPL = maxDrawdownPercentUpToMaxPL,
                        MaxDrawDownDateTimeBeforeMaxPL = maxDrawDownUpToMaxPLDateTime,
                        DaysTillMaxDrawdownBeforeMaxPL = daysTillMaxDrawDownUpToMaxPL,
                        BidPriceEOD = o.BidPriceEOD,
                        AskPriceEOD = o.AskPriceEOD,
                        ClosePriceEOD = o.ClosePriceEOD,
                        BidPriceNextDayOpen = o.BidPriceNextDayOpen,
                        AskPriceNextDayOpen = o.AskPriceNextDayOpen,
                        ClosePriceNextDayOpen = o.ClosePriceNextDayOpen,
                        NextDayOpenCandleDateTime = o.NextDayOpenCandleDateTime
                    };
                }
                catch (Exception ex)
                {
                    throw;
                }

            });


            var summaryFilePath = @"D:\MARKETDATA\LARGEORDEROUTPUT\ANALYSIS";
            var date = DateTime.Now;
            var fileName = "largorderanalysis_output_" + string.Format("{0:yyMMddmmss}", date) + ".csv";
            using (var streamWriter = new StreamWriter(Path.Combine(summaryFilePath, fileName)))
            {
                using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                {
                    csvWriter.WriteRecords(osbAnalysis);
                }
                //Console.WriteLine("Symbol - Time - NotionalValue - BuySellStatus - Expiration - Price - Qty - MaxPL - MaxPLPercent - ");
                //foreach (var osb in osbAnalysis)
                //{
                //    Console.WriteLine($"{osb.o.Symbol} - {osb.o.Time} - {osb.o.NotionalValue} - {osb.o.BuySellStatus} - {osb.o.Expiration} - {osb.o.Price} - {osb.o.Qty} - {osb.maxPL} - {osb.maxPLPercent}");
                //}

                //Console.WriteLine($"Average Max PL - {osbAnalysis.Average(o => o.maxPLPercent)}");
            }
        }

        private async static Task GenerateCsv()
        {
            var context = new BackTestContext();
            CandleDates = context.OptionCandles.Select(o => o.Datetime).Distinct().ToList();

            for (int i = 0; i < 1 ; i++)
            {
                await GenerateTop100ZScorePositions(i);
            }

            Console.WriteLine("Press any key and enter twice to exit");
            Console.ReadLine();
            Console.ReadLine();
        }

        private async static Task GenerateTop100ZScorePositions(int daysAgo)
        {
            var context = new BackTestContext();



            //var positions = context.Positions.ToList();

            //var candles = context.Set<OptionCandle>().FromSqlInterpolated($"SELECT oc.*, p.Id AS PositionId FROM Positions p JOIN OptionCandles oc ON p.OptionContractId = oc.OptionContractId").ToList();




            var now = DateTime.UtcNow;
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var open = DateTimeUtilities.GetMarketOpen(daysAgo, DateTime.UtcNow);
            var close = DateTimeUtilities.GetMarketClose(daysAgo, DateTime.UtcNow);

            var openMs = (long)open.Subtract(epoch).TotalMilliseconds;
            var closeMs = (long)close.Subtract(epoch).TotalMilliseconds;


            // ZScore = (timesales_1109201843[NotionalValue] - timesales_1109201843[AvgDollarValue]) / timesales_1109201843[StDevSymb]
            var timeSales = await context.OptionTimeSales.Where(o => o.Time >= openMs && o.Time <= closeMs).Include(o => o.OptionContract).ToListAsync();
            timeSales = timeSales.Where(t => t.OptionContract.Expiration.Subtract(open).TotalDays <= 40).ToList();

            var group = timeSales.GroupBy(t => t.OptionContract.UnderlyingSymbol);

            var symbolStats = group.Select(g => new
            {
                UnderlyingSymbol = g.Key,
                StDev = Statistics.StandardDeviation(g.Select(k => k.Qty * k.Price * 100)),
                AvgQty = g.Average(k => k.Qty),
                AvgNotionalValue = g.Average(k => k.Qty * k.Price)
            }).ToDictionary(g => g.UnderlyingSymbol, g => g);


            var optionTimeSaleStats = timeSales.Select(t =>
            {
                var symbolStat = symbolStats[t.OptionContract.UnderlyingSymbol];
                return new OptionTimeSaleStatistic
                {
                    OptionTimeSale = t,
                    ZScore = ((t.NotionalValue) - symbolStat.AvgNotionalValue) / symbolStat.StDev
                };
            }).ToList();

            var xlTxs = optionTimeSaleStats.Where(o => o.ZScore != double.PositiveInfinity).OrderByDescending(o => o.OptionTimeSale.NotionalValue).Where(o => o.OptionTimeSale.NotionalValue >= 500000 || o.OptionTimeSale.Qty >= 500).ToList();
            var timeSaleUpdates = new List<OptionTimeSale>();

            // Now get first candle from next day
            var nextDayOpen = DateTimeUtilities.GetMarketOpen(daysAgo - 1, DateTime.UtcNow);

            var longTomOpenDate = long.Parse(string.Format("{0:yyyyMMdd}0000", nextDayOpen.Date));
            var longTomDate = long.Parse(string.Format("{0:yyyyMMdd}0000", nextDayOpen.Date));
            var longTodDate = long.Parse(string.Format("{0:yyyyMMdd}0000", open.Date));
            var tomorrowDate = CandleDates.OrderBy(o => o).FirstOrDefault(o => o >= longTomOpenDate);
            var todayDate = CandleDates.Where(o => o < longTomDate).OrderByDescending(o => o).First(o => o > longTodDate);
            //var todayEndDate = CandleDates
            var todayCandleList = await context.OptionCandles.Where(o => o.Datetime == todayDate).ToListAsync();
            //var todayEndCandleList = await context.OptionCandles.Where(o => o.Datetime == todayEndDate).ToListAsync();

            var todayCandles = new Dictionary<int, OptionCandle>();
            todayCandleList.ForEach(c => {
                if (!todayCandles.ContainsKey(c.OptionContractId))
                {
                    todayCandles[c.OptionContractId] = c;
                }
            });

            var tomorrowCandleList = await context.OptionCandles.Where(o => o.Datetime == tomorrowDate).ToListAsync();
            var tomorrowCandles = new Dictionary<int, OptionCandle>();
            tomorrowCandleList.ForEach(c => {
                if (!tomorrowCandles.ContainsKey(c.OptionContractId))
                {
                    tomorrowCandles[c.OptionContractId] = c;
                }
            });


            var positionSummaries = new List<LargeTradeSummary>();

            var optionTimeSaleStatsGrouped = optionTimeSaleStats.GroupBy(o => o.OptionTimeSale.Time).ToDictionary(o => o.Key, o => o.ToList());

            foreach (var xlTx in xlTxs)
            {
                if (positionSummaries.Any(p => p.OptionTimeSaleId == xlTx.OptionTimeSale.Id))
                {
                    continue;
                }

                //if (!tomorrowCandles.ContainsKey(xlTx.OptionTimeSale.OptionContractId))
                //{
                //    continue;
                //}

                if (!todayCandles.ContainsKey(xlTx.OptionTimeSale.OptionContractId))
                {
                    continue;
                }

                var tomorrowCandle = new OptionCandle();

                if (tomorrowCandles.ContainsKey(xlTx.OptionTimeSale.OptionContractId))
                {
                    tomorrowCandle = tomorrowCandles[xlTx.OptionTimeSale.OptionContractId];
                }

                var todayCandle = todayCandles[xlTx.OptionTimeSale.OptionContractId];

                var optionTimeSaleStatsTime = optionTimeSaleStatsGrouped[xlTx.OptionTimeSale.Time];
                var legs = optionTimeSaleStatsTime.Where(o => o.OptionTimeSale.OptionContract.UnderlyingSymbol == xlTx.OptionTimeSale.OptionContract.UnderlyingSymbol && (o.OptionTimeSale.Qty > (xlTx.OptionTimeSale.Qty / 10)) && o.OptionTimeSale.Id != xlTx.OptionTimeSale.Id && todayCandles.ContainsKey(o.OptionTimeSale.OptionContractId) && tomorrowCandles.ContainsKey(o.OptionTimeSale.OptionContractId)).ToList(); //&& top1.OptionTimeSale.Qty == o.OptionTimeSale.Qty);

                var openInterestChange = tomorrowCandle.OpenInterest - todayCandle.OpenInterest;

                var status = GetTradeOpenCloseStatus(tomorrowCandle, todayCandle, xlTx.OptionTimeSale);

                var positionSummary = new LargeTradeSummary
                {
                    Timestamp = xlTx.OptionTimeSale.Time,
                    Symbol = xlTx.OptionTimeSale.OptionContract.Symbol,
                    Expiration = xlTx.OptionTimeSale.OptionContract.Expiration,
                    OptionContractId = xlTx.OptionTimeSale.OptionContract.Id,
                    OptionTimeSaleId = xlTx.OptionTimeSale.Id,
                    Price = xlTx.OptionTimeSale.Price,
                    Qty = xlTx.OptionTimeSale.Qty,
                    NotionalValue = xlTx.OptionTimeSale.NotionalValue,
                    OpenedClosed = status,
                    OpenInterestChange = openInterestChange,
                    Bid = xlTx.OptionTimeSale.Bid,
                    Ask = xlTx.OptionTimeSale.Ask,
                    IsParentTrade = true,
                    SingleLeg = legs.Count > 0,
                    UnderlyingPrice = xlTx.OptionTimeSale.UnderlyingPrice,
                    DayVolume = todayCandle.Volume,
                    UnderlyingPriceClose = todayCandle.UnderlyingPrice,
                    UnderlyingPriceNextDayOpen = tomorrowCandle.UnderlyingPrice,
                    BidPriceEOD = todayCandle.Bid,
                    AskPriceEOD = todayCandle.Ask,
                    ClosePriceEOD = todayCandle.Close,
                    BidPriceNextDayOpen = tomorrowCandle.Bid,
                    AskPriceNextDayOpen = tomorrowCandle.Ask,
                    ClosePriceNextDayOpen = tomorrowCandle.Close,
                    NextDayOpenCandleDateTime = tomorrowCandle.Datetime

                };

                var legSummaries = legs.Select(l =>
                {

                    var tomorrowCandle = new OptionCandle();

                    if (tomorrowCandles.ContainsKey(l.OptionTimeSale.OptionContractId))
                    {
                        tomorrowCandle = tomorrowCandles[l.OptionTimeSale.OptionContractId];
                    }

                    var todayCandle = todayCandles[l.OptionTimeSale.OptionContractId];
                    var openInterestChange = tomorrowCandle.OpenInterest - todayCandle.OpenInterest;

                    var openClosed = GetTradeOpenCloseStatus(tomorrowCandle, todayCandle, l.OptionTimeSale);

                    return new LargeTradeSummary
                    {
                        Symbol = l.OptionTimeSale.OptionContract.Symbol,
                        Expiration = l.OptionTimeSale.OptionContract.Expiration,
                        Timestamp = l.OptionTimeSale.Time,
                        OptionContractId = l.OptionTimeSale.OptionContractId,
                        OptionTimeSaleId = l.OptionTimeSale.Id,
                        Price = l.OptionTimeSale.Price,
                        Qty = l.OptionTimeSale.Qty,
                        NotionalValue = l.OptionTimeSale.NotionalValue,
                        OpenedClosed = openClosed,
                        OpenInterestChange = openInterestChange,
                        Bid = l.OptionTimeSale.Bid,
                        Ask = l.OptionTimeSale.Ask,
                        IsParentTrade = false,
                        LargeTradeSummaryParentTradeId = xlTx.OptionTimeSale.Id,
                        SingleLeg = false,
                        UnderlyingPrice = l.OptionTimeSale.UnderlyingPrice,
                        DayVolume = todayCandle.Volume,
                        UnderlyingPriceClose = todayCandle.UnderlyingPrice,
                        UnderlyingPriceNextDayOpen = tomorrowCandle.UnderlyingPrice,
                        BidPriceEOD = todayCandle.Bid,
                        AskPriceEOD = todayCandle.Ask,
                        ClosePriceEOD = todayCandle.Close,
                        BidPriceNextDayOpen = tomorrowCandle.Bid,
                        AskPriceNextDayOpen = tomorrowCandle.Ask,
                        ClosePriceNextDayOpen = tomorrowCandle.Close,
                        NextDayOpenCandleDateTime = tomorrowCandle.Datetime

                    };
                }).ToList();


                positionSummaries.Add(positionSummary);

                if (legSummaries.Count > 0)
                {
                    positionSummaries.AddRange(legSummaries);
                }


            }

            var summaryFilePath = @"D:\MARKETDATA\LARGEORDEROUTPUT";
            var date = DateTime.Now;
            var fileName = "largorder_output_notv500k_qty500" + string.Format("{0:yyMMdd}", date) + daysAgo.ToString() + ".csv";
            using (var streamWriter = new StreamWriter(Path.Combine(summaryFilePath, fileName)))
            {
                using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                {
                    csvWriter.WriteRecords(positionSummaries);
                }




                //foreach (var update in timeSaleUpdates)
                //{

                //    var test = context.Entry(update);
                //    test.Property(t => t.TradeId).IsModified = true;
                //}

                //context.SaveChanges();

                //var positions = timeSaleUpdates.Select(t => new Position { PortfolioId = 1, OptionContractId = t.OptionContractId, TradePrice = t.Price, Quantity = t.Qty, TradeId = t.TradeId, TradeDate = epoch.AddMilliseconds(t.Time) });

                //context.AddRange(positions);
                //context.SaveChanges();

                //Console.WriteLine($"Saved {positions.Count()} positions");
            }

            // await context.LargeTrades.AddRangeAsync(positionSummaries);
            // await context.SaveChangesAsync();
        }

        private static string GetTradeOpenCloseStatus(OptionCandle tomorrowCandle, OptionCandle todayCandle, OptionTimeSale largeOrder)
        {

            var openInterestChange = tomorrowCandle.OpenInterest - todayCandle.OpenInterest;
            var positionOpenedRequirement = .75 * largeOrder.Qty;
            var positionClosedRequirement = -.75 * largeOrder.Qty;

            if (openInterestChange > positionOpenedRequirement)
            {
                return "Opened";
            }
            else if (openInterestChange < positionClosedRequirement)
            {
                return "Closed";
            }

            return "Undetermined";
        }
    }
}