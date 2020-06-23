using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TDMarketData.Service;

[assembly: FunctionsStartup(typeof(TDMarketDataFunctionApp.TDMarketDataFunctionAppStartup))]

namespace TDMarketDataFunctionApp
{
    public class TDMarketDataFunctionAppStartup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {

            var config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                                .AddEnvironmentVariables()
                                .Build();



            builder.Services.AddOptions();
            var tdAppSettings = new TDMarketData.Service.TDApiSettings();
            var tdAppConfig = config.GetSection(nameof(TDApiSettings));
            tdAppConfig.Bind(tdAppSettings);

            builder.Services.Configure<TDApiSettings>(tdAppConfig);

            var tdAuthToken = new TDAuthToken();

            if (!string.IsNullOrEmpty(tdAppSettings.LastAccessToken))
            {
                tdAuthToken.access_token = tdAppSettings.LastAccessToken;
                tdAuthToken.expires_in = tdAppSettings.LastExpires;
                tdAuthToken.refresh_token = tdAuthToken.refresh_token;
            }

            var httpClient = new TDHttpClient(tdAppSettings, tdAuthToken)
            {
                BaseAddress = new Uri(tdAppSettings.BaseAddress)
            };

            if (!string.IsNullOrEmpty(tdAppSettings.LastAccessToken))
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tdAppSettings.LastAccessToken);
            }


            builder.Services.AddSingleton(httpClient);
            builder.Services.AddSingleton(tdAuthToken);
            builder.Services.AddScoped<TDMarketDataService>();

        }
    }
}

