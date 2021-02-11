using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TDMarketData.BackTesting.Data;
using TDMarketData.BackTesting.Data.Helpers;
using TDMarketData.BackTesting.Data.Models;
using TDMarketData.Domain;
using TDMarketData.Domain.TDStreaming;
using TDMarketData.PositionGenerator;
using TDMarketData.Service.TDApi;
using TDMarketData.Service.Utilities;

namespace TDMarketData.StreamingWebApp
{
    public class TDWebSocketConnectHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly FileStream _optionBidAskFileStream;
        private readonly FileStream _equityTimeSaleFileStream;
        private readonly FileStream _wssLog;
        private long EquityTimeSaleReceiveCount = 0;
        private long OptionBidAskReceiveCount = 0;


        public TDWebSocketConnectHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            var dateTime = DateTime.Now.ToString("yyMMddhhmm");
            _optionBidAskFileStream = new FileStream(@"D:\MARKETDATA\OPTIONBIDASK\wss_option_bid_ask_" + dateTime, FileMode.OpenOrCreate);

            _equityTimeSaleFileStream = new FileStream(@"D:\MARKETDATA\EQUITYTIMESALE\wss_equity_timesale_" + dateTime, FileMode.OpenOrCreate);

            _wssLog = new FileStream(@"D:\MARKETDATA\LOG\wss_log_" + dateTime, FileMode.OpenOrCreate);
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                try
                {

                    var tdUserPrincipalService = scope.ServiceProvider.GetService<TDUserPrincipalService>();
                    using (ClientWebSocket ws = new ClientWebSocket())
                    {
                        var userPrincipal = await tdUserPrincipalService.GetUserPrincipal("streamerSubscriptionKeys,streamerConnectionInfo");

                        var websocketUrl = "ws://" + userPrincipal.StreamerInfo.StreamerSocketUrl + "/ws";
                        Uri serverUri = new Uri(websocketUrl);

                        await ws.ConnectAsync(serverUri, CancellationToken.None);
                        await Login(ws, userPrincipal);
                        await SubscribeOptionBidAsk(ws, userPrincipal);
                        await SubscribeEquityTimeSale(ws, userPrincipal);

                        while (ws.State == WebSocketState.Open)
                        {
                            await ReceiveAsync(ws);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error: {ex.StackTrace}");
                }

            }
        }

        private async Task SubscribeEquityTimeSale(ClientWebSocket ws, TDUserPrincipal userPrincipal)
        {
            var top50 = Helpers.GetTop50OptionVolumeSymbols();

            var equiyTimeSaleRequest = TDStreamingUtilities.GetEquityTimeSaleStreamingRequest(top50, userPrincipal.Accounts[0].AccountId, userPrincipal.StreamerInfo.AppId);


            var streamMessage = new JObject();
            var requestArray = new JArray();
            requestArray.Add(JToken.FromObject(equiyTimeSaleRequest));

            streamMessage["requests"] = requestArray;
            var streamReq = streamMessage.ToString();
            ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(streamReq));
            await ws.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);


        }

        public async Task Login(ClientWebSocket ws, TDUserPrincipal userPrincipal)
        {
            var loginMessage = TDStreamingUtilities.GetAdminLoginRequest(userPrincipal);

            var streamMessage = new JObject();
            var requestArray = new JArray();

            requestArray.Add(JToken.FromObject(loginMessage));
            streamMessage["requests"] = requestArray;


            await SendAsync(ws, streamMessage);
            await ReceiveAsync(ws);
        }

        private async Task SendAsync(ClientWebSocket ws, JObject streamMessage)
        {
            var message = streamMessage.ToString();
            ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
            await ws.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task SubscribeOptionBidAsk(WebSocket ws, TDUserPrincipal userPrincipal)
        {

            // read optiontickers and subscribe to bid ask 
            var context = new BackTestContext();
            var currentUnderlyingPrices = await SqlHelpers.GetCurrentUnderlying(context);
            var top50 = Helpers.GetTop50OptionVolumeSymbols();
            var today = DateTime.UtcNow.Date;
            var optionTickersTop50 = context.OptionContracts.Where(o => top50.Contains(o.UnderlyingSymbol) && o.Expiration >= today).ToList();
            var tickers = new List<OptionContract>();
            foreach (var symbol in top50)
            {
                var optionTickers = optionTickersTop50.Where(o => o.UnderlyingSymbol == symbol);

                if (!currentUnderlyingPrices.ContainsKey(symbol))
                {
                    continue;
                }

                var optionCurrentData = currentUnderlyingPrices[symbol];

                var volatility = 0D;
                if (!double.TryParse(optionCurrentData.Volatility, out volatility))
                {
                    continue;
                }

                if (Double.IsNaN(volatility))
                {
                    continue;
                }

                // add 3 items to tickers 1 atm, 1 stdev above and below current price, 2 weeks till expiration

                // add atm option 
                var twoWeeks = DateTimeUtilities.GetMarketOpen(-10, DateTime.UtcNow).Date;
                var oneWeek = DateTimeUtilities.GetMarketOpen(-5, DateTime.UtcNow).Date;
                var twoWeekExpiry = optionTickers.Select(o => o.Expiration).OrderBy(o => o).First(o => o >= twoWeeks);
                var oneWeekExpiry = optionTickers.Select(o => o.Expiration).OrderBy(o => o).First(o => o >= oneWeek);


                var twoWeekOptionContracts = optionTickers.Where(o => o.Expiration == twoWeekExpiry);
                var oneWeekOptionContracts = optionTickers.Where(o => o.Expiration == oneWeekExpiry);

                var oneWeekTickers = GetItmOtmAtmPerExpiration(oneWeekOptionContracts, oneWeekExpiry, optionCurrentData);
                var twoWeekTickers = GetItmOtmAtmPerExpiration(twoWeekOptionContracts, twoWeekExpiry, optionCurrentData);

                tickers.AddRange(oneWeekTickers);
                tickers.AddRange(twoWeekTickers);

            }

            var count = 0;
            var streamMessage = new JObject();
            var requestArray = new JArray();

            var transformedOptionTickers = new List<string>();
            foreach (var optionTicker in tickers)
            {
                var symbol = optionTicker.Symbol;
                symbol = symbol.Replace(".", "");
                var expirationStart = TDOptionSymbolHelpers.GetExpirationStartIndex(symbol);

                symbol = symbol.Substring(0, expirationStart) + "_" + symbol.Substring(expirationStart, symbol.Length - expirationStart);

                expirationStart = TDOptionSymbolHelpers.GetExpirationStartIndex(symbol);

                var oldexpiration = symbol.Substring(expirationStart, 6);

                var expiration = oldexpiration.Substring(2, 4) + oldexpiration.Substring(0, 2);

                symbol = symbol.Replace(oldexpiration, expiration);

                transformedOptionTickers.Add(symbol);

                if (count < 2)
                {
                    transformedOptionTickers.Add(symbol);

                }
                count++;
            }
            var optionTimeSaleRequest = TDStreamingUtilities.GetLevel1OptionStreamingRequest(transformedOptionTickers.ToArray(), userPrincipal.Accounts[0].AccountId, userPrincipal.StreamerInfo.AppId);
            requestArray.Add(JToken.FromObject(optionTimeSaleRequest));

            streamMessage["requests"] = requestArray;
            var streamReq = streamMessage.ToString();
            ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(streamReq));
            await ws.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private List<OptionContract> GetItmOtmAtmPerExpiration(IEnumerable<OptionContract> optionContracts, DateTime expiry, UnderlyingData optionCurrentData)
        {
            var tickers = new List<OptionContract>();
            var atmCallContract = GetClosestStrikeContract(optionCurrentData.UnderlyingPrice, optionContracts, 'C');
            var atmPutContract = GetClosestStrikeContract(optionCurrentData.UnderlyingPrice, optionContracts, 'P');

            var daysTillExpiration = expiry.Subtract(DateTime.UtcNow.Date).TotalDays;

            var stFct = double.Parse(optionCurrentData.Volatility) * Math.Sqrt(daysTillExpiration / 365);
            var stup = optionCurrentData.UnderlyingPrice + (stFct * optionCurrentData.UnderlyingPrice);
            var stdown = optionCurrentData.UnderlyingPrice - (stFct * optionCurrentData.UnderlyingPrice);

            var stUpCallContract = GetClosestStrikeContract(stup, optionContracts, 'C');
            var stUpPutContract = GetClosestStrikeContract(stup, optionContracts, 'P');

            var stDownCallContract = GetClosestStrikeContract(stdown, optionContracts, 'C');
            var stDownPutContract = GetClosestStrikeContract(stdown, optionContracts, 'P');

            tickers.Add(atmCallContract);
            tickers.Add(atmPutContract);
            tickers.Add(stUpCallContract);
            tickers.Add(stUpPutContract);
            tickers.Add(stDownCallContract);
            tickers.Add(stDownPutContract);

            return tickers;
        }

        private static OptionContract GetClosestStrikeContract(double closeTo, IEnumerable<OptionContract> optionContracts, char callPut)
        {
            var optionContractStrikeDiff = optionContracts.Select(o => new { o, StrikeDiff = Math.Round(Math.Abs(o.Strike - closeTo), 4) });
            var min = optionContractStrikeDiff.Min(o => o.StrikeDiff);
            var atmContract = optionContractStrikeDiff.FirstOrDefault(o => o.StrikeDiff == min && o.o.CallPut == callPut);

            if (atmContract == null)
            {
                return null;
            }
            return atmContract.o;
        }

        public static byte[] Combine(byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
        }

        public async Task<JObject> ReceiveAsync(WebSocket ws)
        {
            var output = new JObject();
            _equityTimeSaleFileStream.Flush();
           _optionBidAskFileStream.Flush();
            var stringOffWire = string.Empty;
            try
            {

                var bytesReceived = new byte[65536];
                byte[] newline = Encoding.Default.GetBytes(Environment.NewLine);

                WebSocketReceiveResult result = await ws.ReceiveAsync(bytesReceived, CancellationToken.None);

                while (!result.EndOfMessage)
                {
                    var next = new byte[65536];
                    result = await ws.ReceiveAsync(next, CancellationToken.None);

                    bytesReceived = Combine(bytesReceived, next);
                }

                stringOffWire = Encoding.Default.GetString(bytesReceived);

                var jObj = JObject.Parse(stringOffWire);

                var jData = jObj["data"] as JArray;

                if (jData != null)
                {
                    foreach (var jToken in jData)
                    {
                        var service = jToken["service"].ToString();

                        var bytes = Encoding.Default.GetBytes(jToken.ToString(Formatting.None));

                        if (service == "OPTION")
                        {
                            OptionBidAskReceiveCount++;
                            var flush = false;

                            if (OptionBidAskReceiveCount % 100 == 0)
                            {
                                Debug.WriteLine($"Received {OptionBidAskReceiveCount} option B/A");
                                flush = true;
                            }

                            var newLine = Encoding.Default.GetBytes(Environment.NewLine);
                            await _optionBidAskFileStream.WriteAsync(bytes);
                            await _optionBidAskFileStream.WriteAsync(newLine);

                            if (flush)
                                await _optionBidAskFileStream.FlushAsync();
                        } 
                        else if (service == "TIMESALE_EQUITY")
                        {
                            EquityTimeSaleReceiveCount++;
                            var flush = false;
                            if (EquityTimeSaleReceiveCount % 100 == 0)
                            {
                                Debug.WriteLine($"Received {EquityTimeSaleReceiveCount} timesale equity");
                                flush = true;
                            }

                            var newLine = Encoding.Default.GetBytes(Environment.NewLine);

                            await _equityTimeSaleFileStream.WriteAsync(bytes);
                            await _equityTimeSaleFileStream.WriteAsync(newLine);

                            if (flush)
                                await _equityTimeSaleFileStream.FlushAsync();

                        }
                        else
                        {
                            await _wssLog.WriteAsync(bytes);
                            await _wssLog.FlushAsync();
                        }
                    }
                }
            } catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.StackTrace}, string : {stringOffWire}");
                throw;
            }

            return output;
        }
        public async override Task StopAsync(CancellationToken cancellationToken)
        {

            await _optionBidAskFileStream.FlushAsync();

            await base.StopAsync(cancellationToken);
        }
    }
}
