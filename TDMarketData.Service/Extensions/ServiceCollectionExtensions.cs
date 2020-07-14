using Azure.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using TDMarketData.Service.DataStorage;
using TDMarketData.Service.TDApi;

namespace TDMarketData.Service.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static void RegisterServices(this IServiceCollection services, IConfiguration config)
        {
            var tdAppSettings = new TDApiSettings();
            var tdAppConfig = config.GetSection("TDApiSettings");

            if (tdAppConfig.Exists())
            {
                if (!string.IsNullOrEmpty(tdAppConfig.Value))
                {
                    var jObject = JObject.Parse(tdAppConfig.Value);
                    tdAppSettings = jObject.ToObject<TDApiSettings>();
                }
                else
                {
                    tdAppConfig.Bind(tdAppSettings);
                }
            }

            services.AddSingleton(tdAppSettings);

            var tdAuthToken = new TDAuthToken();

            if (!string.IsNullOrEmpty(tdAppSettings.LastAccessToken))
            {
                tdAuthToken.access_token = tdAppSettings.LastAccessToken;
                tdAuthToken.expires_in = tdAppSettings.LastExpires;
                tdAuthToken.refresh_token = tdAuthToken.refresh_token;
            }


            services.AddSingleton<TDHttpClient>();
            services.AddSingleton(tdAuthToken);
            services.AddScoped<TDMarketDataService>();
            services.AddScoped<MarketDataTableStorageService>();
            services.AddSingleton<MarketDataFileStorageService>();
            services.AddScoped<TDUserPrincipalService>();

            var tableStorageConfig = config.GetSection(nameof(StorageApiSettings));

            var tableStorageApiSettings = new StorageApiSettings();

            if (tableStorageConfig.Exists())
            {
                if (!string.IsNullOrEmpty(tableStorageConfig.Value))
                {
                    var jObject = JObject.Parse(tableStorageConfig.Value);
                    tableStorageApiSettings = jObject.ToObject<StorageApiSettings>();
                }
                else
                {
                    tableStorageConfig.Bind(tableStorageApiSettings);
                }
            }

            services.AddSingleton(tableStorageApiSettings);
        }
    }
}
