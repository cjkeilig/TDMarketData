﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TDMarketData.Domain.TableStorageDto
{
    public class Candle
    {
        public string Symbol { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public long Volume { get; set; }
        public string Datetime { get; set; }
    }
}
