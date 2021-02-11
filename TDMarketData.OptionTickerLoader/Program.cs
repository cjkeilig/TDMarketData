using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TDMarketData.BackTesting.Data;
using TDMarketData.BackTesting.Data.Helpers;
using TDMarketData.BackTesting.Data.Models;
using TDMarketData.PositionGenerator;

namespace TDMarketData.OptionTickerLoader
{
    class Program
    {
        //private static DateTime DateFileFilter { get; set; }
        private static DateTime[] DateTimeArr { get; set; }
        static void Main(string[] args)
        {

            Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File(@"D:\MARKETDATA\LOG\ETL.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
 
            //DateFileFilter = new DateTime(2020, 11, 2);

            Run().GetAwaiter().GetResult();
        }

        private static async Task Run()
        {

            // if during market hours due time is 0
            //var candlePeriod = 5 * 60 * 1000;
            //var candleTimer = new Timer(async (object state) => await LoadOptionCandles(), null, 0, candlePeriod);


            //var today = DateTime.Now;
            //DateTime runAt;
            //if (today.Hour >= 19)
            //{
            //    var tomorrow = today.AddDays(1);
            //    runAt = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 19, 0, 0);
            //}
            //else
            //{
            //    runAt = new DateTime(today.Year, today.Month, today.Day, 19, 0, 0);
            //}

            //var dailyPeriod = 1000 * 60 * 60 * 24;
            //var dailyDueTime = (int)runAt.Subtract(today).TotalMilliseconds;


            //var dailyNightTimer = new Timer(async (object state) => await LoadDailyData(), null, dailyDueTime, dailyPeriod);

            DateTimeArr = new DateTime[] {
                //new DateTime(2020, 10, 5),
                //new DateTime(2020, 10, 6),
                //new DateTime(2020, 10, 7),
                //new DateTime(2020, 10, 8),
                //new DateTime(2020, 10, 9),

                //new DateTime(2020, 10, 12),
                //new DateTime(2020, 10, 13),
                //new DateTime(2020, 10, 14),
                //new DateTime(2020, 10, 15),
                //new DateTime(2020, 10, 16),

                //new DateTime(2020, 10, 19),
                //new DateTime(2020, 10, 20),
                //new DateTime(2020, 10, 21),
                //new DateTime(2020, 10, 22),
                //new DateTime(2020, 10, 23),

                //new DateTime(2020, 10, 26),
                //new DateTime(2020, 10, 27),
                //new DateTime(2020, 10, 28),
                //new DateTime(2020, 10, 29),
                //new DateTime(2020, 10, 30),

                //new DateTime(2020, 11, 2),
                //new DateTime(2020, 11, 3),
                //new DateTime(2020, 11, 4),
                //new DateTime(2020, 11, 5),
                //new DateTime(2020, 11, 6),

                //new DateTime(2020, 11, 9),
                //new DateTime(2020, 11, 10),
                new DateTime(2020, 12, 9)
            };
            

            foreach (var date in DateTimeArr)
            {
                await LoadDailyData(date);
            }

            Console.WriteLine("Press a key and enter twice to stop");
            Console.ReadLine();
            Console.ReadLine();

            //var context = new BackTestContext();
            //var stats = await context.DailyStats.Include(d => d.StatDetail).Where(d => d.Date == DateFileFilter).ToListAsync();
            //await GenerateStatCsv(stats);
        }

        private static async Task LoadDailyData(DateTime date)
        {
            Log.Debug($"In LoadDailyData {date}");

         await LoadOptionContracts(date);

          await LoadOptionCandles(date);
          var optionTimeSales = await LoadOptionTimeSales(date);

         await LoadOptionDailyStats(optionTimeSales, date);

         await BackLoadOptionDailyStatDetails(date);

            //var context = new BackTestContext();

            //var stats = await context.DailyStats.Include(d => d.StatDetail).Where(d => d.Date == date).ToListAsync();
            //await GenerateStatCsv(stats);

        }

        private async static Task GenerateStatCsv(List<OptionSymbolDailyStat> stats)
        {
            // generate csv with stat and detail row, add columns for percent change from last day
            // notional value, call/put notional value, total volume, volatility, vwap

            var statFilePath = @"D:\MARKETDATA\STATOUTPUT";
            var date = stats.First().Date;
            var fileName = "stat_output_" + string.Format("{0:yyMMdd}", date) + ".csv";

            var context = new BackTestContext();

            var previousDay = DateTimeUtilities.GetMarketOpen(0, date).Date;
            var previousDayStats = await context.DailyStats.Include(d => d.StatDetail).Where(d => d.Date == previousDay).ToListAsync();

            using (var streamWriter = new StreamWriter(Path.Combine(statFilePath, fileName)))
            {
                using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                {
                    WriteHeaderRecord(csvWriter);

                    csvWriter.NextRecord();

                    foreach (var stat in stats)
                    {
                        var previousDayStat = previousDayStats.FirstOrDefault(p => p.symbol == stat.symbol);

                        if (previousDay == null)
                        {
                            continue;
                        }

                        csvWriter.WriteField(stat.Id);
                        csvWriter.WriteField(stat.symbol);
                        csvWriter.WriteField(stat.iv);
                        csvWriter.WriteField(stat.totalVol);
                        csvWriter.WriteField(stat.putVol);
                        csvWriter.WriteField(stat.callVol);
                        csvWriter.WriteField(stat.PutNotionalValue);
                        csvWriter.WriteField(stat.CallNotionalValue);

                        csvWriter.WriteField(stat.CallNotionalValue + stat.PutNotionalValue);
                        csvWriter.WriteField(stat.vwap);

                        csvWriter.WriteField((double.TryParse(previousDayStat.iv, out double prevIv) && prevIv != 0D) ? (((double.TryParse(stat.iv, out double iv) ? iv : 0) - prevIv) / prevIv * 100) : 0D);
                        csvWriter.WriteField((double.TryParse(previousDayStat.totalVol, out double prevTotalVol) && prevTotalVol != 0D) ? (((double.TryParse(stat.totalVol, out double totalVol) ? totalVol : 0) - prevTotalVol) / prevTotalVol * 100) : 0D);
                        csvWriter.WriteField((double.TryParse(previousDayStat.putVol, out double prevPutVol) && prevPutVol != 0D) ? (((double.TryParse(stat.putVol, out double putVol) ? putVol : 0) - prevPutVol) / prevPutVol * 100) : 0D);
                        csvWriter.WriteField((double.TryParse(previousDayStat.callVol, out double prevCallVol) && prevCallVol != 0D) ? (((double.TryParse(stat.callVol, out double callVol) ? callVol : 0) - prevCallVol) / prevCallVol * 100) : 0D);
                        csvWriter.WriteField(Convert.ToDouble(previousDayStat.PutNotionalValue) != 0D ? (Convert.ToDouble(stat.PutNotionalValue) - Convert.ToDouble(previousDayStat.PutNotionalValue)) / Convert.ToDouble(previousDayStat.PutNotionalValue) * 100 : 0D);
                        csvWriter.WriteField(Convert.ToDouble(previousDayStat.CallNotionalValue) != 0D ? (Convert.ToDouble(stat.CallNotionalValue) - Convert.ToDouble(previousDayStat.CallNotionalValue)) / Convert.ToDouble(previousDayStat.CallNotionalValue) * 100 : 0D);
                        var yesterdayNotValue = (double)previousDayStat.PutNotionalValue + (double)previousDayStat.CallNotionalValue;
                        csvWriter.WriteField((yesterdayNotValue != 0D) ? (((double)stat.CallNotionalValue + (double)stat.PutNotionalValue - yesterdayNotValue) / yesterdayNotValue * 100D) : 0D);
                        csvWriter.WriteField((double.TryParse(previousDayStat.vwap, out double prevVwap) && prevVwap != 0D) ? (((double.TryParse(stat.vwap, out double vwap) ? vwap : 0) - prevVwap) / prevVwap * 100) : 0D);

                        var putCallRatio = double.TryParse(stat.callVol, out double callVolH) && callVolH != 0D ? double.TryParse(stat.putVol, out double putVolH) ? putVolH / callVolH : 0D : 0D;
                        csvWriter.WriteField(putCallRatio);

                        var oldPutCallRatio = double.TryParse(previousDayStat.callVol, out double callVolP) && callVolP != 0D ? double.TryParse(previousDayStat.putVol, out double putVolP) ? putVolP / callVolP : 0D : 0D;
                        var putCallRatioPctChg = putCallRatio != 0D ? oldPutCallRatio != 0 ? (putCallRatio - oldPutCallRatio) / oldPutCallRatio * 100 : 0D : 0D;
                        csvWriter.WriteField(putCallRatioPctChg);
                        csvWriter.WriteField(stat.Date);
                        csvWriter.WriteField(stat.StatDetail.Id);
                        csvWriter.WriteField(stat.StatDetail.NotionalValue5DayMvgAvg);
                        csvWriter.WriteField(stat.StatDetail.PutCallRatio5DayMvgAvg);
                        csvWriter.WriteField(stat.StatDetail.Volume5DayMvgAvg);
                        csvWriter.WriteField(stat.StatDetail.Volatility5DayMvgAvg);
                        csvWriter.WriteField(stat.StatDetail.NotionalValueXLPerc10);
                        csvWriter.WriteField(stat.StatDetail.NotionalValueXLPerc20);
                        csvWriter.WriteField(stat.StatDetail.NotionalValueXLPerc30);
                        csvWriter.WriteField(stat.StatDetail.NotionalValueXLPerc40);
                        csvWriter.WriteField(stat.StatDetail.NotionalValueXLPerc50);
                        csvWriter.WriteField(stat.StatDetail.NotionalValueXLPerc60);
                        csvWriter.WriteField(stat.StatDetail.NotionalValueXLPerc70);
                        csvWriter.WriteField(stat.StatDetail.NotionalValueXLPerc80);
                        csvWriter.WriteField(stat.StatDetail.NotionalValueXLPerc90);
                        csvWriter.WriteField(previousDayStat.StatDetail.NotionalValue5DayMvgAvg != 0D ? ((stat.StatDetail.NotionalValue5DayMvgAvg - previousDayStat.StatDetail.NotionalValue5DayMvgAvg) / previousDayStat.StatDetail.NotionalValue5DayMvgAvg) * 100 : 0);
                        csvWriter.WriteField(previousDayStat.StatDetail.PutCallRatio5DayMvgAvg != 0D ? ((stat.StatDetail.PutCallRatio5DayMvgAvg - previousDayStat.StatDetail.PutCallRatio5DayMvgAvg) / previousDayStat.StatDetail.PutCallRatio5DayMvgAvg) * 100 : 0);
                        csvWriter.WriteField(previousDayStat.StatDetail.Volume5DayMvgAvg != 0D ? ((stat.StatDetail.Volume5DayMvgAvg - previousDayStat.StatDetail.Volume5DayMvgAvg) / previousDayStat.StatDetail.Volume5DayMvgAvg) * 100 : 0);
                        csvWriter.WriteField(previousDayStat.StatDetail.Volatility5DayMvgAvg != 0D ? ((stat.StatDetail.Volatility5DayMvgAvg - previousDayStat.StatDetail.Volatility5DayMvgAvg) / previousDayStat.StatDetail.Volatility5DayMvgAvg) * 100 : 0);
                        csvWriter.WriteField(previousDayStat.StatDetail.NotionalValueXLPerc10 != 0D ? ((stat.StatDetail.NotionalValueXLPerc10 - previousDayStat.StatDetail.NotionalValueXLPerc10) / previousDayStat.StatDetail.NotionalValueXLPerc10) * 100 : 0);
                        csvWriter.WriteField(previousDayStat.StatDetail.NotionalValueXLPerc20 != 0D ? ((stat.StatDetail.NotionalValueXLPerc20 - previousDayStat.StatDetail.NotionalValueXLPerc20) / previousDayStat.StatDetail.NotionalValueXLPerc20) * 100 : 0);
                        csvWriter.WriteField(previousDayStat.StatDetail.NotionalValueXLPerc30 != 0D ? ((stat.StatDetail.NotionalValueXLPerc30 - previousDayStat.StatDetail.NotionalValueXLPerc30) / previousDayStat.StatDetail.NotionalValueXLPerc30) * 100 : 0);
                        csvWriter.WriteField(previousDayStat.StatDetail.NotionalValueXLPerc40 != 0D ? ((stat.StatDetail.NotionalValueXLPerc40 - previousDayStat.StatDetail.NotionalValueXLPerc40) / previousDayStat.StatDetail.NotionalValueXLPerc40) * 100 : 0);
                        csvWriter.WriteField(previousDayStat.StatDetail.NotionalValueXLPerc50 != 0D ? ((stat.StatDetail.NotionalValueXLPerc50 - previousDayStat.StatDetail.NotionalValueXLPerc50) / previousDayStat.StatDetail.NotionalValueXLPerc50) * 100 : 0);
                        csvWriter.WriteField(previousDayStat.StatDetail.NotionalValueXLPerc60 != 0D ? ((stat.StatDetail.NotionalValueXLPerc60 - previousDayStat.StatDetail.NotionalValueXLPerc60) / previousDayStat.StatDetail.NotionalValueXLPerc60) * 100 : 0);
                        csvWriter.WriteField(previousDayStat.StatDetail.NotionalValueXLPerc70 != 0D ? ((stat.StatDetail.NotionalValueXLPerc70 - previousDayStat.StatDetail.NotionalValueXLPerc70) / previousDayStat.StatDetail.NotionalValueXLPerc70) * 100 : 0);
                        csvWriter.WriteField(previousDayStat.StatDetail.NotionalValueXLPerc80 != 0D ? ((stat.StatDetail.NotionalValueXLPerc80 - previousDayStat.StatDetail.NotionalValueXLPerc80) / previousDayStat.StatDetail.NotionalValueXLPerc80) * 100 : 0);
                        csvWriter.WriteField(previousDayStat.StatDetail.NotionalValueXLPerc90 != 0D ? ((stat.StatDetail.NotionalValueXLPerc90 - previousDayStat.StatDetail.NotionalValueXLPerc90) / previousDayStat.StatDetail.NotionalValueXLPerc90) * 100 : 0);

                        csvWriter.NextRecord();

                    }

                }
            }
        }

        private static void WriteHeaderRecord(CsvWriter csvWriter)
        {
            csvWriter.WriteField("OptionSymbolDailyStatId");
            csvWriter.WriteField(nameof(OptionSymbolDailyStat.symbol));
            csvWriter.WriteField(nameof(OptionSymbolDailyStat.iv));
            csvWriter.WriteField(nameof(OptionSymbolDailyStat.totalVol));
            csvWriter.WriteField(nameof(OptionSymbolDailyStat.putVol));
            csvWriter.WriteField(nameof(OptionSymbolDailyStat.callVol));
            csvWriter.WriteField(nameof(OptionSymbolDailyStat.PutNotionalValue));
            csvWriter.WriteField(nameof(OptionSymbolDailyStat.CallNotionalValue));
            csvWriter.WriteField("NotionalValue");
            csvWriter.WriteField(nameof(OptionSymbolDailyStat.vwap));
            csvWriter.WriteField("ivPctChg");
            csvWriter.WriteField("totalVolPctChg");
            csvWriter.WriteField("putVolPctChg");
            csvWriter.WriteField("callVolPctChg");
            csvWriter.WriteField("PutNotionalValuePctChg");
            csvWriter.WriteField("CallNotionalValuePctChg");
            csvWriter.WriteField("NotionalValuePctChg");
            csvWriter.WriteField("vwapPctChg");
            csvWriter.WriteField("putCallRatio");
            csvWriter.WriteField("putCallRatioPctChg");
            csvWriter.WriteField(nameof(OptionSymbolDailyStat.Date));

            csvWriter.WriteField("OptionSymbolDailyStatDetailId");
            csvWriter.WriteField(nameof(OptionSymbolDailyStatDetail.NotionalValue5DayMvgAvg));
            csvWriter.WriteField(nameof(OptionSymbolDailyStatDetail.PutCallRatio5DayMvgAvg));
            csvWriter.WriteField(nameof(OptionSymbolDailyStatDetail.Volume5DayMvgAvg));
            csvWriter.WriteField(nameof(OptionSymbolDailyStatDetail.Volatility5DayMvgAvg));
            csvWriter.WriteField(nameof(OptionSymbolDailyStatDetail.NotionalValueXLPerc10));
            csvWriter.WriteField(nameof(OptionSymbolDailyStatDetail.NotionalValueXLPerc20));
            csvWriter.WriteField(nameof(OptionSymbolDailyStatDetail.NotionalValueXLPerc30));
            csvWriter.WriteField(nameof(OptionSymbolDailyStatDetail.NotionalValueXLPerc40));
            csvWriter.WriteField(nameof(OptionSymbolDailyStatDetail.NotionalValueXLPerc50));
            csvWriter.WriteField(nameof(OptionSymbolDailyStatDetail.NotionalValueXLPerc60));
            csvWriter.WriteField(nameof(OptionSymbolDailyStatDetail.NotionalValueXLPerc70));
            csvWriter.WriteField(nameof(OptionSymbolDailyStatDetail.NotionalValueXLPerc80));
            csvWriter.WriteField(nameof(OptionSymbolDailyStatDetail.NotionalValueXLPerc90));

            csvWriter.WriteField("NotionalValue5DayMvgAvgPctChg");
            csvWriter.WriteField("PutCallRatio5DayMvgAvgPctChg");
            csvWriter.WriteField("Volume5DayMvgAvgPctChg");
            csvWriter.WriteField("Volatility5DayMvgAvgPctChg");
            csvWriter.WriteField("NotionalValueXLPerc10PctChg");
            csvWriter.WriteField("NotionalValueXLPerc20PctChg");
            csvWriter.WriteField("NotionalValueXLPerc30PctChg");
            csvWriter.WriteField("NotionalValueXLPerc40PctChg");
            csvWriter.WriteField("NotionalValueXLPerc50PctChg");
            csvWriter.WriteField("NotionalValueXLPerc60PctChg");
            csvWriter.WriteField("NotionalValueXLPerc70PctChg");
            csvWriter.WriteField("NotionalValueXLPerc80PctChg");
            csvWriter.WriteField("NotionalValueXLPerc90PctChg");
        }

        private async static Task<List<OptionSymbolDailyStatDetail>> BackLoadOptionDailyStatDetails(DateTime date)
        {
            var context = new BackTestContext();
            var existingStats = await context.DailyStats.Where(d => d.Date <= date).ToListAsync();

            var earliestStat = existingStats.Min(e => e.Date);
            var existingStatDays = existingStats.Select(e => e.Date).Distinct().OrderBy(e => e);

            var dayStatDetailList = new List<OptionSymbolDailyStatDetail>();

            var dayGroup = existingStats.Max(e => e.Date);

            var statDaysLessThanOrEqualTo5 = existingStatDays.Where(e => e <= dayGroup).OrderByDescending(e => e).Take(5).ToList();
            var statDaysLessThanOrEqualTo20 = existingStatDays.Where(e => e <= dayGroup).OrderByDescending(e => e).Take(20).ToList();
            var statDaysLessThanOrEqualTo30 = existingStatDays.Where(e => e <= dayGroup).OrderByDescending(e => e).Take(30).ToList();

            //if (statDaysLessThanOrEqualToS.Count < 5)
            //{
            //    return dayStatDetailList;
            //}
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var marketOpen = DateTimeUtilities.GetMarketOpen(dayGroup.Year, dayGroup.Month, dayGroup.Day, date);
            var marketClose = DateTimeUtilities.GetMarketClose(dayGroup.Year, dayGroup.Month, dayGroup.Day, date);
            var startTime = (long)marketOpen.Subtract(epoch).TotalMilliseconds;
            var endTime = (long)marketClose.Subtract(epoch).TotalMilliseconds;


            try
            {

                var optionTimeSales = context.OptionTimeSales.Where(o => o.Time >= startTime && o.Time <= endTime).Include(o => o.OptionContract).ToList().GroupBy(o => o.OptionContract.UnderlyingSymbol).ToDictionary(o => o.Key, o => o.ToList());



            var output = new List<OptionSymbolDailyStatDetail>();

            var start5DayWindowDate = statDaysLessThanOrEqualTo5.Min();
            var end5DayWindowDate = statDaysLessThanOrEqualTo5.Max();
            var window5DayStats = existingStats.Where(e => e.Date >= start5DayWindowDate && e.Date <= end5DayWindowDate);

            var start20DayWindowDate = statDaysLessThanOrEqualTo20.Min();
            var end20DayWindowDate = statDaysLessThanOrEqualTo20.Max();
            var window20DayStats = existingStats.Where(e => e.Date >= start20DayWindowDate && e.Date <= end20DayWindowDate);

            var start30DayWindowDate = statDaysLessThanOrEqualTo30.Min();
            var end30DayWindowDate = statDaysLessThanOrEqualTo30.Max();
            var window30DayStats = existingStats.Where(e => e.Date >= start30DayWindowDate && e.Date <= end30DayWindowDate);

            var dayStatDetails = window5DayStats.GroupBy(s => s.symbol).Select(g =>
            {
                var stat = existingStats.First(e => e.Date == dayGroup && e.symbol == g.Key);
                var statDetail = new OptionSymbolDailyStatDetail();
                statDetail.OptionSymbolDailyStatId = stat.Id;
                statDetail.PutCallRatio5DayMvgAvg = g.Average(s => double.TryParse(s.callVol, out double callVol) ? ((double.TryParse(s.putVol, out double putVol) ? putVol : 0D) / callVol) : 0D);
                statDetail.Volume5DayMvgAvg = g.Average(s => double.TryParse(s.totalVol, out double totalVol) ? totalVol : 0D);
                statDetail.Volatility5DayMvgAvg = g.Average(s => double.TryParse(s.iv, out double iv) ? iv : 0D);
                statDetail.NotionalValue5DayMvgAvg = g.Average(s => s.CallNotionalValue + s.PutNotionalValue);


                var twentyDayStats = window20DayStats.Where(s => s.symbol == g.Key).ToList();

                if (twentyDayStats.Count == 20)
                {
                    statDetail.PutCallRatio20DayMvgAvg = twentyDayStats.Average(s => double.TryParse(s.callVol, out double callVol) ? ((double.TryParse(s.putVol, out double putVol) ? putVol : 0D) / callVol) : 0D);
                    statDetail.Volume20DayMvgAvg = twentyDayStats.Average(s => double.TryParse(s.totalVol, out double totalVol) ? totalVol : 0D);
                    statDetail.Volatility20DayMvgAvg = twentyDayStats.Average(s => double.TryParse(s.iv, out double iv) ? iv : 0D);
                    statDetail.NotionalValue20DayMvgAvg = twentyDayStats.Average(s => s.CallNotionalValue + s.PutNotionalValue);
                }


                var thirtyDayStats = window30DayStats.Where(s => s.symbol == g.Key).ToList();

                if (thirtyDayStats.Count == 30)
                {
                    statDetail.PutCallRatio30DayMvgAvg = thirtyDayStats.Average(s => double.TryParse(s.callVol, out double callVol) ? ((double.TryParse(s.putVol, out double putVol) ? putVol : 0D) / callVol) : 0D);
                    statDetail.Volume30DayMvgAvg = thirtyDayStats.Average(s => double.TryParse(s.totalVol, out double totalVol) ? totalVol : 0D);
                    statDetail.Volatility30DayMvgAvg = thirtyDayStats.Average(s => double.TryParse(s.iv, out double iv) ? iv : 0D);
                    statDetail.NotionalValue30DayMvgAvg = thirtyDayStats.Average(s => s.CallNotionalValue + s.PutNotionalValue);
                }



                if (!optionTimeSales.ContainsKey(g.Key))
                    return statDetail;  //, g.Key };

                var symbolTimeSales = optionTimeSales[g.Key].Select(o => new { o.Price, o.Qty, o.Id, NotionalValue = o.Price * o.Qty * 100 }).OrderByDescending(o => o.NotionalValue).ToList();
                var totalNotionalValue = symbolTimeSales.Sum(s => s.NotionalValue);

                var total = 0D;
                statDetail.NotionalValueXLPerc10 = (symbolTimeSales.TakeWhile(s =>
                {
                    total += s.NotionalValue; return total < (totalNotionalValue * .1);
                }).Count() / (double)symbolTimeSales.Count) * 100;

                total = 0D;
                statDetail.NotionalValueXLPerc20 = (symbolTimeSales.TakeWhile(s =>
                {
                    total += s.NotionalValue; return total < (totalNotionalValue * .2);
                }).Count() / (double)symbolTimeSales.Count) * 100;

                total = 0D;
                statDetail.NotionalValueXLPerc30 = (symbolTimeSales.TakeWhile(s =>
                {
                    total += s.NotionalValue; return total < (totalNotionalValue * .3);
                }).Count() / (double)symbolTimeSales.Count) * 100;

                total = 0D;
                statDetail.NotionalValueXLPerc40 = (symbolTimeSales.TakeWhile(s =>
                {
                    total += s.NotionalValue; return total < (totalNotionalValue * .4);
                }).Count() / (double)symbolTimeSales.Count) * 100;

                total = 0D;
                statDetail.NotionalValueXLPerc50 = (symbolTimeSales.TakeWhile(s =>
                {
                    total += s.NotionalValue; return total < (totalNotionalValue * .5);
                }).Count() / (double)symbolTimeSales.Count) * 100;

                total = 0D;
                statDetail.NotionalValueXLPerc60 = (symbolTimeSales.TakeWhile(s =>
                {
                    total += s.NotionalValue; return total < (totalNotionalValue * .6);
                }).Count() / (double)symbolTimeSales.Count) * 100;

                total = 0D;
                statDetail.NotionalValueXLPerc70 = (symbolTimeSales.TakeWhile(s =>
                {
                    total += s.NotionalValue; return total < (totalNotionalValue * .7);
                }).Count() / (double)symbolTimeSales.Count) * 100;

                total = 0D;
                statDetail.NotionalValueXLPerc80 = (symbolTimeSales.TakeWhile(s =>
                {
                    total += s.NotionalValue; return total < (totalNotionalValue * .8);
                }).Count() / (double)symbolTimeSales.Count) * 100;

                total = 0D;
                statDetail.NotionalValueXLPerc90 = (symbolTimeSales.TakeWhile(s =>
                {
                    total += s.NotionalValue; return total < (totalNotionalValue * .9);

                }).Count() / (double)symbolTimeSales.Count) * 100;

                return statDetail;

            }).ToList();


            dayStatDetailList.AddRange(dayStatDetails);
            // var topcNot10Perc = dayStatDetails.OrderByDescending(d => d.statDetail.NotionalValueXLPerc10).ToList();
            //topcNot10Perc.ForEach(t => Log.Debug($"not10perc {t.Key}, {t.statDetail.NotionalValueXLPerc10}"));

            // var topcNot20Perc = dayStatDetails.OrderByDescending(d => d.statDetail.NotionalValueXLPerc20).ToList();
            // topcNot20Perc.ForEach(t => Log.Debug($"not20perc {t.Key}, {t.statDetail.NotionalValueXLPerc20}"));

            //var topcNot30Perc = dayStatDetails.OrderByDescending(d => d.statDetail.NotionalValueXLPerc30).ToList();
            // topcNot30Perc.ForEach(t => Log.Debug($"not30perc {t.Key}, {t.statDetail.NotionalValueXLPerc30}"));

            // Log.Debug($"create details for {dayGroup} : putcallratio5 > 0: {dayStatDetails.Where(d => d.statDetail.PutCallRatio5DayMvgAvg > 0).ToList().Count}");

            //}

            await context.AddRangeAsync(dayStatDetailList);
            await context.SaveChangesAsync();


            }
            catch (Exception ex)
            {

            }

            return dayStatDetailList;
        }

        private async static Task<List<OptionSymbolDailyStat>> LoadOptionDailyStats(List<OptionTimeSale> optionTimeSales, DateTime date)
        {
            var backLoading = optionTimeSales.Count == 0;
            var optionTimeSalesFilePath = @"D:\MARKETDATA\OPTIONSTATS";

            var fileSearchFilter = "stats_" + string.Format("{0:D2}{1:D2}{2}", date.Day, date.Month, date.Year.ToString().Substring(2, 2)) + "*";
            var files = Directory.GetFiles(optionTimeSalesFilePath, fileSearchFilter).ToList();

            var stats = new List<OptionSymbolDailyStat>();

            foreach (var file in files)
            {
                var context = new BackTestContext();

                var fileName = Path.GetFileName(file);


                var fileStatus = context.FileStatus.FirstOrDefault(f => f.FileName == fileName);

                if (fileStatus != null)
                {
                    continue;
                }
                else
                {
                    fileStatus = new FileStatus
                    {
                        FileName = fileName,
                        Status = "Starting"
                    };

                    await context.FileStatus.AddAsync(fileStatus);
                    await context.SaveChangesAsync();
                }



                var fileDateStr = fileName.Substring(6, 6);
                var fileDate = new DateTime(int.Parse("20" + fileDateStr.Substring(4, 2)), int.Parse(fileDateStr.Substring(2, 2)), int.Parse(fileDateStr.Substring(0, 2)));
                var records = new List<OptionSymbolDailyStat>();

                using (var reader = new StreamReader(file))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Configuration.RegisterClassMap<OptionSymbolDailyStatClassMap>();
                    csv.Configuration.HeaderValidated = null;
                    records = csv.GetRecords<OptionSymbolDailyStat>().ToList();

                    var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    var fileDateUtc = fileDate.AddHours(5);
                    while (fileDateUtc.Hour < 22)
                    {
                        fileDateUtc = fileDateUtc.AddHours(1);
                    }
                    var st = DateTimeUtilities.GetMarketOpen(0, fileDateUtc);

                    records.ForEach(r => r.Date = st.Date);

                    var startTime = st.Subtract(epoch).TotalMilliseconds;
                    var endTime = DateTimeUtilities.GetMarketClose(0, fileDateUtc).Subtract(epoch).TotalMilliseconds;
                    if (backLoading)
                    {
                        optionTimeSales = context.OptionTimeSales.Where(o => o.Time >= startTime && o.Time <= endTime).Include(o => o.OptionContract).ToList();
                    }

                    var otsGrouped = optionTimeSales.GroupBy(o => new { o.OptionContract.UnderlyingSymbol, o.OptionContract.CallPut });

                    foreach (var group in otsGrouped)
                    {
                        try
                        {


                            var stat = records.FirstOrDefault(r => r.symbol == group.Key.UnderlyingSymbol);

                            if (stat is null)
                            {
                                continue;
                            }

                            if (group.Key.CallPut == 'C')
                            {
                                stat.CallNotionalValue = (long)group.Sum(g => g.Qty * g.Price * 100);
                            }

                            if (group.Key.CallPut == 'P')
                            {
                                stat.PutNotionalValue = (long)group.Sum(g => g.Qty * g.Price * 100);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                    }

                    await context.DailyStats.AddRangeAsync(records);
                    await context.SaveChangesAsync();

                    Log.Debug($"Saved {records.Count} stats");
                }

                stats.AddRange(records);

                await CompleteFileProcessing(context, fileStatus);

            }

            return stats;
        }

        private static async Task CompleteFileProcessing(BackTestContext context, FileStatus fileStatus)
        {

            fileStatus.Status = "Completed";

            var fileStatusEntry = context.Entry(fileStatus);

            fileStatusEntry.Property(f => f.Status).IsModified = true;

            await context.SaveChangesAsync();

        }

        private async static Task<List<OptionTimeSale>> LoadOptionTimeSales(DateTime date)
        {
            var optionTimeSalesFilePath = @"D:\MARKETDATA\TIMESALES\";
            var realTimeFolder = "realtime";

            var fileSearchFilter = "timesales_" + string.Format("{0:D2}{1:D2}{2}", date.Day, date.Month, date.Year.ToString().Substring(2, 2)) + "*";
            var files = Directory.GetFiles(optionTimeSalesFilePath, fileSearchFilter).ToList();

            var realTimeFiles = Directory.GetFiles(Path.Combine(optionTimeSalesFilePath, realTimeFolder), fileSearchFilter);

            files.AddRange(realTimeFiles);

            var optionTimeSalesToSave = new List<OptionTimeSale>();

            var context2 = new BackTestContext();

            var optionContracts = context2.OptionContracts.AsNoTracking().ToDictionary(o => o.Symbol, o => o);
            var processedTImeSales = new List<OptionTimeSale>();
            foreach (var file in files)
            {
                var optionTimeSales = new List<OptionTimeSale>();


                var fileStatus = context2.FileStatus.FirstOrDefault(f => f.FileName == file);

                if (fileStatus != null)
                {
                    continue;
                }
                else
                {
                    fileStatus = new FileStatus { FileName = file, Status = "Starting" };
                    await context2.FileStatus.AddAsync(fileStatus);
                    await context2.SaveChangesAsync();
                }

                using (var reader = new StreamReader(file))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Configuration.RegisterClassMap<OptionTimeSaleMap>();
                    var records = csv.GetRecords<OptionTimeSale>();

                    optionTimeSales.AddRange(records);

                    foreach (var timeSale in optionTimeSales)
                    {

                        if (optionContracts.TryGetValue(timeSale.Symbol, out OptionContract optionContract))
                        {
                            timeSale.OptionContractId = optionContract.Id;
                            timeSale.OptionContract = optionContract;
                        }
                    }

                    optionTimeSalesToSave = optionTimeSales.Where(o => o.OptionContractId > 0).ToList();


                    if (optionTimeSalesToSave.Count > 0)
                    {
                        await BatchSaveOptionTimeSales(optionTimeSalesToSave);
                        Log.Debug($"Saved {optionTimeSalesToSave.Count} timesales");

                        processedTImeSales.AddRange(optionTimeSalesToSave);
                    }



                }

                await CompleteFileProcessing(context2, fileStatus);
            }

            Log.Debug($"Saved {processedTImeSales.Count} timesales");

            return processedTImeSales;

        }

        private static async Task BatchSaveOptionTimeSales(List<OptionTimeSale> optionTimeSalesToSave)
        {
            int saved = 0;
            var total = optionTimeSalesToSave.Count;
            while (saved < total)
            {
                var next = optionTimeSalesToSave.Skip(saved).Take(1000000).ToList();
                var context = new BackTestContext();
                context.ChangeTracker.AutoDetectChangesEnabled = false;

                try
                {
                    await context.OptionTimeSales.AddRangeAsync(next);

                    optionTimeSalesToSave.ForEach(o => context.Entry(o.OptionContract).State = EntityState.Unchanged);
                    var test = await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw;
                }

                saved += next.Count;

            }
        }

        private static async Task LoadOptionCandles(DateTime date)
        {
            Log.Debug("In LoadOptionCandles");

            var optionCandleFilePath = @"D:\MARKETDATA\OPTIONCANDLES";
            var fileSearchFilter = "option_candle_" + string.Format("{0:D2}{1:D2}{2}", date.Month, date.Day, date.Year.ToString().Substring(2, 2)) + "*"; //DateFileFilter.Month, DateFileFilter.Day, DateFileFilter.Year.ToString().Substring(2, 2)) + "*";

            var files = Directory.GetFiles(optionCandleFilePath, fileSearchFilter);

            var context2 = new BackTestContext();

            var optionContracts = context2.OptionContracts.AsNoTracking().Select(o => new { o.Id, o.Symbol }).ToDictionary(o => o.Symbol, o => o.Id);

            foreach (var file in files)
            {
                var fileStatus = context2.FileStatus.FirstOrDefault(f => f.FileName == file);

                try
                {


                    var optionCandles = new List<OptionCandle>();


                    if (fileStatus != null)
                    {
                        continue;
                    }
                    else
                    {
                        fileStatus = new FileStatus { FileName = file, Status = "Starting" };
                        await context2.FileStatus.AddAsync(fileStatus);
                        await context2.SaveChangesAsync();
                    }

                    using (var reader = new StreamReader(file))
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        csv.Configuration.RegisterClassMap<OptionCandleMap>();
                        var records = csv.GetRecords<OptionCandle>();

                        optionCandles.AddRange(records);

                        foreach (var candle in optionCandles)
                        {
                            var symbol = candle.Symbol.Replace("_", "");
                            symbol = "." + symbol;

                            var expiration = TDOptionSymbolHelpers.GetExpiration(symbol);

                            var newExpiration = expiration.Substring(4, 2) + expiration.Substring(0, 4);


                            var optionSymbol = symbol.Replace(expiration, newExpiration);

                            if (optionContracts.TryGetValue(optionSymbol, out int optionContractId))
                            {
                                candle.OptionContractId = optionContractId;
                            }
                        }

                        var optionCandlesToSave = optionCandles.Where(o => o.OptionContractId > 0).ToList();

                        if (optionCandlesToSave.Count > 0)
                        {
                            var context = new BackTestContext();
                            context.ChangeTracker.AutoDetectChangesEnabled = false;


                            await context.OptionCandles.AddRangeAsync(optionCandlesToSave);
                            await context.SaveChangesAsync();

                            Log.Debug($"Saved {optionCandlesToSave.Count} candles");
                        }
                    }
                }
                catch (Exception ex)
                {
                    fileStatus.Status = $"Error: {ex.Message}";
                    Log.Error($"Error processing file: {file} ex.StackTrace");
                }

                await CompleteFileProcessing(context2, fileStatus);
            }
        }

        private static async Task LoadOptionContracts(DateTime date)
        {
            var optionTickerFilePath = @"D:\MARKETDATA\OPTIONTICKERS";

            var fileSearchFilter = "optiontickers_" + string.Format("{0:D2}{1:D2}{2}", date.Day, date.Month, date.Year.ToString().Substring(2, 2)) + "*";

            var files = Directory.GetFiles(optionTickerFilePath, fileSearchFilter);

            var optionTickersRaw = new List<string>();
            var fileStatuses = new List<FileStatus>();

            foreach (var file in files)
            {
                //var fileStatus = new FileStatus
                //{
                //    FileName = file,
                //    Status = "Starting"
                //};
                //fileStatuses.Add(fileStatus);

                var reader = new StreamReader(file);

                while (!reader.EndOfStream)
                {
                    optionTickersRaw.Add(reader.ReadLine());
                }

                reader.Close();

            }
            var context = new BackTestContext();

           // context.FileStatus.AddRange(fileStatuses);
          //  await context.SaveChangesAsync();

            var optionSymbols = context.OptionContracts.Select(o => o.Symbol).ToList();

            var newOptionContracts = optionTickersRaw.Except(optionSymbols);

            try
            {


                var newOptionContractEntities = newOptionContracts.Select(o =>
                {
                    try
                    {


                        var callPut = TDOptionSymbolHelpers.GetCallPut(o);

                        var strike = TDOptionSymbolHelpers.GetStrike(o);

                        var expiration = TDOptionSymbolHelpers.GetExpiration(o);

                        var expDate = new DateTime(int.Parse("20" + expiration.Substring(0, 2)), int.Parse(expiration.Substring(2, 2)), int.Parse(expiration.Substring(4, 2)));

                        var underlyingSymbol = TDOptionSymbolHelpers.GetUnderlyingSymbol(o);
                        return new OptionContract
                        {
                            CallPut = callPut,
                            Strike = strike,
                            Expiration = expDate,
                            UnderlyingSymbol = underlyingSymbol,
                            Symbol = o
                        };

                    }
                    catch (Exception ex)
                    {
                        return null;
                    }


                }).ToList();

                if (newOptionContractEntities.Count > 0)
                {
                    await context.AddRangeAsync(newOptionContractEntities);
                    await context.SaveChangesAsync();

                    Log.Debug($"Saved {newOptionContractEntities.Count} contracts");

                }
            }
            catch (Exception ex)
            {

            }

           // fileStatuses.ForEach(async (f) => await CompleteFileProcessing(context, f));
        }
    }
}