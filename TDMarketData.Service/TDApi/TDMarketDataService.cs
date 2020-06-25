using System;
using System.Runtime.CompilerServices;
using TDMarketData.Domain;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace TDMarketData.Service
{
    public class TDMarketDataService
    {
        private readonly TDHttpClient _tdHttpClient;
        private readonly TDApiSettings _tdApiSettings;
        public TDMarketDataService(TDHttpClient tdHttpClient, TDApiSettings tdApiSettings)
        {
            _tdHttpClient = tdHttpClient;
            _tdApiSettings = tdApiSettings;
        }
        public async Task<TDOptionChain> GetOptionChain(TDOptionChainRequest tdOptionChainRequest)
        {
            await _tdHttpClient.EnsureAuthenticated();

            var fullUrl = _tdApiSettings.OptionChainUri + "?symbol=" + tdOptionChainRequest.Symbol;

            if (tdOptionChainRequest.StrikeCount > 0)
            {
                fullUrl += "&strikeCount=" + tdOptionChainRequest.StrikeCount;
            }

            var optionChainResponse = await _tdHttpClient.GetAsync(fullUrl);

            var optionChain = JsonConvert.DeserializeObject<TDOptionChain>(await optionChainResponse.Content.ReadAsStringAsync());

            return optionChain;
        }

        public async Task<List<TDQuote>> GetQuote(string symbol)
        {
            await _tdHttpClient.EnsureAuthenticated();

            var response = await _tdHttpClient.GetAsync(string.Format(_tdApiSettings.QuoteUri, symbol));

            var val = await response.Content.ReadAsStringAsync();

            var jobject = JObject.Parse(val);

            var jQuotes = jobject.Children().Select(q => q.Children());

            var realQuotes = jQuotes.SelectMany(q => q.Select(qt => qt.ToObject<TDQuote>()));

            return realQuotes.ToList();
        }

        public async Task<List<TDCandle>> GetPriceHistory(TDPriceHistoryRequest tdPriceHistoryRequest)
        {
            await _tdHttpClient.EnsureAuthenticated();

            var uri = string.Format(_tdApiSettings.PriceHistoryUri, tdPriceHistoryRequest.Symbol);

            var queryString = "?period=" + tdPriceHistoryRequest.Period + "&periodType=" + tdPriceHistoryRequest.PeriodType + "&frequency=" + tdPriceHistoryRequest.Frequency + "&frequencyType=" + tdPriceHistoryRequest.FrequencyType;

            var fullUri = uri + queryString;

            var response = await _tdHttpClient.GetAsync(fullUri);

            var content = await response.Content.ReadAsStringAsync();

            var jContent = JObject.Parse(content);

            var jCandles = jContent.Value<JToken>("candles");

            var candles = jCandles.Select(c => c.ToObject<TDCandle>());

            return candles.ToList();

            
        }
            

    }
}
