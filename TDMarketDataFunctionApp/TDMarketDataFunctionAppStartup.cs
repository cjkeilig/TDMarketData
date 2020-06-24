using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TDMarketData.Service;
using AutoMapper;
using TDMarketData.Service.DataStorage;
using Newtonsoft.Json.Linq;

[assembly: FunctionsStartup(typeof(TDMarketDataFunctionApp.TDMarketDataFunctionAppStartup))]

namespace TDMarketDataFunctionApp
{
    public class TDMarketDataFunctionAppStartup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            Console.WriteLine("in configure");
            var config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                                .AddEnvironmentVariables()
                                .Build();



            builder.Services.AddOptions();
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

            builder.Services.AddSingleton(tdAppSettings);

            var tdAuthToken = new TDAuthToken();

            if (!string.IsNullOrEmpty(tdAppSettings.LastAccessToken))
            {
                tdAuthToken.access_token = tdAppSettings.LastAccessToken;
                tdAuthToken.expires_in = tdAppSettings.LastExpires;
                tdAuthToken.refresh_token = tdAuthToken.refresh_token;
            }

         
            builder.Services.AddSingleton<TDHttpClient>();
            builder.Services.AddSingleton(tdAuthToken);
            builder.Services.AddScoped<TDMarketDataService>();
            builder.Services.AddScoped<MarketDataStorageService>();

            builder.Services.AddAutoMapper(new System.Reflection.Assembly[] { typeof(MarketDataMapperProfile).Assembly });

            var tableStorageConfig = config.GetSection(nameof(TableStorageApiSettings));

            var tableStorageApiSettings = new TableStorageApiSettings();

            if (tableStorageConfig.Exists())
            {
                tableStorageConfig.Bind(tableStorageApiSettings);
            }
            else
            {
                var tableStorageJObject = JObject.Parse(GetEnvironmentVariable(nameof(TableStorageApiSettings)));
                tableStorageApiSettings = tableStorageJObject.ToObject<TableStorageApiSettings>();
            }

            builder.Services.AddSingleton(tableStorageApiSettings);

        }

        public string GetEnvironmentVariable(string variable)
        {
            return Environment.GetEnvironmentVariable(variable);
        }
    }
}

