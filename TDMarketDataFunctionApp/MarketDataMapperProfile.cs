using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TDMarketData.Domain;
using TDMarketData.Domain.TableStorageDto;

namespace TDMarketDataFunctionApp
{
    public class MarketDataMapperProfile : Profile
    {
        public MarketDataMapperProfile()
        {
            CreateMap<TDCandle, Candle>().ForMember(td => td.Datetime, opt => opt.MapFrom(src => string.Format("{0:yyyyMMddHHmm}", new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(src.Datetime))));
            CreateMap<OptionExpDateMap, Option>();
        }
    }
}
