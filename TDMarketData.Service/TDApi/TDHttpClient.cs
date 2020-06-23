using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace TDMarketData.Service
{
    public class TDHttpClient : HttpClient
    {
        private readonly TDApiSettings _tdApiSettings;
        private TDAuthToken _tdAuthToken;
        public TDHttpClient(TDApiSettings tdApiSettings, TDAuthToken tdAuthToken)
        {
            _tdApiSettings = tdApiSettings;
            _tdAuthToken = tdAuthToken;
        }


        public async System.Threading.Tasks.Task EnsureAuthenticated()
        {

            if (!string.IsNullOrEmpty(_tdAuthToken.access_token))
            {
                var minutesTokenValid = (_tdAuthToken.expires_in / 60) - _tdApiSettings.RefreshTokenBufferPeriodMinutes;
                var minutesTokenAlive = (DateTime.Now - _tdAuthToken.issued_date).Minutes;
                if (minutesTokenAlive < minutesTokenValid)
                {
                    return;
                }
            }

            Dictionary<string, string> authFormData;
            if (string.IsNullOrEmpty(_tdAuthToken.access_token))
            {
                authFormData = new Dictionary<string, string>
                    {
                        { "client_id", _tdApiSettings.ConsumerKey },
                        { "grant_type", "authorization_code" },
                        { "code", _tdApiSettings.AuthCode },
                        { "access_type", "offline" },
                        { "redirect_uri", "http://localhost:8080" },

                    };
            }
            else
            {
                authFormData = new Dictionary<string, string>
                    {
                        { "client_id", _tdApiSettings.ConsumerKey },
                        { "grant_type", "refresh_token" },
                        { "refresh_token", _tdAuthToken.refresh_token }
                    };
            }

            var content = new FormUrlEncodedContent(authFormData);

            _tdAuthToken = await Authenticate(content);

            DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tdAuthToken.access_token);

        }


        public async System.Threading.Tasks.Task<TDAuthToken> Authenticate(HttpContent content)
        {
            var response = await PostAsync(_tdApiSettings.TokenUri, content);

            var resp = await response.Content.ReadAsStringAsync();

            var tokenResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<TDAuthToken>(resp);
            tokenResponse.issued_date = DateTime.Now;

            return tokenResponse;
        }
    }
}
