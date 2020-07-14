using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TDMarketData.Domain;
using TDMarketData.Domain.TDStreaming;
using TDMarketData.Service.TDApi;
using TDMarketData.Service.Utilities;

namespace TDMarketData.StreamingWebApp
{
    public class TDWebSocketConnectHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        public TDWebSocketConnectHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
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
                        await Subscribe(ws, userPrincipal);

                        while (ws.State == WebSocketState.Open)
                        {
                            var jObject = await ReceiveAsync(ws);

                            //Console.WriteLine(jObject.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {

                }

            }
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
            ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(streamMessage.ToString()));
            await ws.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task Subscribe(WebSocket ws, TDUserPrincipal userPrincipal)
        {
            //var quoteStreamingRequest = TDStreamingUtilities.GetQuoteStreamingRequest(new string[] { "MSFT" }, userPrincipal.Accounts[0].AccountId, userPrincipal.StreamerInfo.AppId);
            var optionTimeSaleRequest = TDStreamingUtilities.GetTimeSaleStreamingRequest(new string[] { "MSFT" }, userPrincipal.Accounts[0].AccountId, userPrincipal.StreamerInfo.AppId);

            var streamMessage = new JObject();
            var requestArray = new JArray();

            requestArray.Add(JToken.FromObject(optionTimeSaleRequest));
            streamMessage["requests"] = requestArray;


            ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(streamMessage.ToString()));
            await ws.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
            await ReceiveAsync(ws);
        }

        public async Task<JObject> ReceiveAsync(WebSocket ws)
        {
            ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[65536]);
            WebSocketReceiveResult result = await ws.ReceiveAsync(bytesReceived, CancellationToken.None);
            var result2 = Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count);
            Console.WriteLine(result2);
            return JObject.Parse(result2);
        }
    }
}
