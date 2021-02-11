using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TDMarketData.BackTesting.Data.Models
{
    public class Position
    {
        public int Id { get; set; }
        public int PortfolioId { get; set; } 
        [JsonIgnore]
        public Portfolio Porfolio { get; set; }
        public int OptionContractId { get; set; }
        public OptionContract OptionContract { get; set; }
        public double TradePrice { get; set; }
        public DateTime TradeDate { get; set; }
        public int Quantity { get; set; }
        public int TradeId { get; set; }
        public ICollection<OptionCandle> OptionCandles { get; set; }
    }
}
