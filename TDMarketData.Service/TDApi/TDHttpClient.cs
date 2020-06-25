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
            _logger.LogInformation(_tdAuthToken.access_token+ ": " + _tdAuthToken.issued_date);
            
            if (!string.IsNullOrEmpty(_tdAuthToken.access_token))
            {
                var minutesTokenValid = (_tdAuthToken.expires_in / 60) - _tdApiSettings.RefreshTokenBufferPeriodMinutes;
                var minutesTokenAlive = (DateTime.Now - _tdAuthToken.issued_date).Minutes;
                if (minutesTokenAlive < minutesTokenValid)
                {
                    return;
                }
            }

            bool refresh = false;
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

                DefaultRequestHeaders.Authorization = null;

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

                refresh = true;
            }

            var content = new FormUrlEncodedContent(authFormData);

            var token = await Authenticate(content);
            var now = DateTime.Now;

            if (!refresh)
            {
                _tdAuthToken.access_token = token.access_token;
                _tdAuthToken.expires_in = token.expires_in;
                _tdAuthToken.refresh_token = token.refresh_token;
                _tdAuthToken.refresh_token_expires_in = token.refresh_token_expires_in;
                _tdAuthToken.scope = token.scope;
                _tdAuthToken.refresh_issued_date = now;

            }
            else
            {
                _tdAuthToken.access_token = token.access_token;
            }

            _tdAuthToken.issued_date = now;

            DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tdAuthToken.access_token);

        }

        public async Task<TDAuthToken> Authenticate(HttpContent content)
        {
            try
            {


                var response = await PostAsync(_tdApiSettings.TokenUri, content);

                var resp = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("TD auth resp: " + resp);
                var tokenResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<TDAuthToken>(resp);
                return tokenResponse;

            }
            catch (Exception ex)
            {
                
                _logger.LogError("{0},{1}", ex.Message, ex.StackTrace);
                throw;
            }

        }
    }
}
