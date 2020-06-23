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
        public TDMarketDataService(TDHttpClient tdHttpClient, IOptions<TDApiSettings> tdApiSettings)
        {
            _tdHttpClient = tdHttpClient;
            _tdApiSettings = tdApiSettings.Value;
        }
        public async Task<OptionChain> GetOptionChain(TDOptionChainRequest tdOptionChainRequest)
        {
            await _tdHttpClient.EnsureAuthenticated();

            var optionChainResponse = await _tdHttpClient.GetAsync(_tdApiSettings.OptionChainUri + "?symbol=" + tdOptionChainRequest.Symbol + "&strikeCount=" + tdOptionChainRequest.StrikeCount);

            var optionChain = JsonConvert.DeserializeObject<OptionChain>(await optionChainResponse.Content.ReadAsStringAsync());

            return optionChain;
        }

        public async Task<List<Quote>> GetQuote(string symbol)
        {
            await _tdHttpClient.EnsureAuthenticated();

            var response = await _tdHttpClient.GetAsync(string.Format(_tdApiSettings.QuoteUri, symbol));

            var val = await response.Content.ReadAsStringAsync();

            var jobject = JObject.Parse(val);

            var jQuotes = jobject.Children().Select(q => q.Children());

            var realQuotes = jQuotes.SelectMany(q => q.Select(qt => qt.ToObject<Quote>()));

            return realQuotes.ToList();
        }


    }
}
