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
using TDMarketData.Service.Extensions;

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
            builder.Services.AddAutoMapper(new System.Reflection.Assembly[] { typeof(MarketDataMapperProfile).Assembly });
            builder.Services.RegisterServices(config);

        }
    }
}

