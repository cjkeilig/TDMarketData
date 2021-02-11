using System;
using System.Collections.Generic;
using System.Text;

namespace TDMarketData.BackTesting.Data.Models
{
    public class Portfolio
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Position> Positions { get; set; }
    }
}
