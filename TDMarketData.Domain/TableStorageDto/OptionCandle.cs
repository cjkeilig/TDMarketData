namespace TDMarketData.Domain.TableStorageDto
{
    public class OptionCandle
    {
        public string Symbol { get; set; }
        public double Close { get; set; }
        public long Volume { get; set; }
        public string Datetime { get; set; }
        public double Volatility { get; set; }
        public long OpenInterest { get; set; }
        public double PercentChange { get; set; }
    }
}