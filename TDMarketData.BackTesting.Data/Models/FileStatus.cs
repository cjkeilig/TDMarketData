using System;
using System.Collections.Generic;
using System.Text;

namespace TDMarketData.BackTesting.Data.Models
{
    public class FileStatus
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Status { get; set; }
        public DateTime ProcessedDt { get; set; }
    }
}
