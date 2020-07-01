using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TDMarketData.Domain;

namespace TDMarketData.Service.TDApi
{
    public class TDUserPrincipalService
    {
        private readonly TDHttpClient _tdHttpClient;
        private readonly TDApiSettings _tdApiSettings;
        public TDUserPrincipalService(TDHttpClient tdHttpClient, TDApiSettings tdApiSettings)
        {
            _tdHttpClient = tdHttpClient;
            _tdApiSettings = tdApiSettings;
        }

        public async Task<TDUserPrincipal> GetUserPrincipal(string fields)
        {
            await _tdHttpClient.EnsureAuthenticated();

            var response = await _tdHttpClient.GetAsync(_tdApiSettings.UserPrincipalsUri + "?fields=" + fields);

            var content = await response.Content.ReadAsStringAsync();

            var userPrincipal = JsonConvert.DeserializeObject<TDUserPrincipal>(content);

            return userPrincipal;
        }
    } 
}
