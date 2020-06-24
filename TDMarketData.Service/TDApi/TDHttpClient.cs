using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TDMarketData.Service
{
    public class TDHttpClient : HttpClient
    {
        private readonly TDApiSettings _tdApiSettings;
        private TDAuthToken _tdAuthToken;
        private readonly ILogger<TDHttpClient> _logger;

        public TDHttpClient(TDApiSettings tdApiSettings, TDAuthToken tdAuthToken, ILogger<TDHttpClient> logger)
        {
            _tdApiSettings = tdApiSettings;
            _tdAuthToken = tdAuthToken;
            _logger = logger;

            BaseAddress = new Uri(_tdApiSettings.BaseAddress);

            if (!string.IsNullOrEmpty(_tdApiSettings.LastAccessToken))
            {
                DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tdApiSettings.LastAccessToken);
            }
        }


        public async Task EnsureAuthenticated()
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
                _logger.LogInformation("Access token from auth code");

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
                _logger.LogInformation("Refresh access token");

                authFormData = new Dictionary<string, string>
                    {
                        { "client_id", _tdApiSettings.ConsumerKey },
                        { "grant_type", "refresh_token" },
                        { "refresh_token", _tdAuthToken.refresh_token }
                    };


                var minutesTokenValid = (_tdAuthToken.refresh_token_expires_in / 60) - _tdApiSettings.RefreshTokenBufferPeriodMinutes;
                var minutesTokenAlive = (DateTime.Now - _tdAuthToken.refresh_issued_date).Minutes;
                if (minutesTokenAlive > minutesTokenValid)
                {
                    _logger.LogInformation("Refresh token expired");
                    authFormData.Add("access_type", "offline");
                }
            }

            var content = new FormUrlEncodedContent(authFormData);

            _tdAuthToken = await Authenticate(content);

            var now = DateTime.Now;
            _tdAuthToken.issued_date = now;

            if (authFormData.ContainsKey("access_type"))
            {
                _tdAuthToken.refresh_issued_date = now;
            }


            DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tdAuthToken.access_token);

        }

        public async Task<TDAuthToken> Authenticate(HttpContent content)
        {
            var response = await PostAsync(_tdApiSettings.TokenUri, content);

            var resp = await response.Content.ReadAsStringAsync();

            var tokenResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<TDAuthToken>(resp);

            return tokenResponse;
        }
    }
}
