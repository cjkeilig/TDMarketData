using System;
using System.Collections.Generic;
using System.Text;
using TDMarketData.PositionGenerator;

namespace TDMarketData.BackTesting.Data.Models
{
    public class LargeTradeSummary
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public DateTime Expiration { get; set; }
        public DateTime Time
        {
            get
            {
                return DateTimeUtilities.TimestampToDateTime(Timestamp);
            }
        }
        public long Timestamp { get; set; }
        public int TradingDaysTillExpiration
        {
            get
            {
                return DateTimeUtilities.GetTradingDaysBetween(Time, Expiration);
            }
        }
        public int OptionContractId { get; set; }
        public double Price { get; set; }
        public double Bid { get; set; }
        public double Ask { get; set; }
        public int Qty { get; set; }
        public double BidPriceEOD { get; set; }
        public double AskPriceEOD { get; set; }
        public double ClosePriceEOD { get; set; }
        public double BidPriceNextDayOpen { get; set; }
        public double AskPriceNextDayOpen { get; set; }
        public double ClosePriceNextDayOpen { get; set; }
        public long NextDayOpenCandleDateTime { get; set; }
        public double NotionalValue { get; set; }
        public long OpenInterestChange { get; set; }
        public string OpenedClosed { get; set; }
        public int OptionTimeSaleId { get; set; }
        public int LargeTradeSummaryParentTradeId { get; set; }
        public bool IsParentTrade { get; set; }
        public double BidAskPricePoint
        {
            get
            {
                return (Price - ((Ask + Bid) / 2)) / ((Ask - Bid) / 2);
            }
        }
        public string BuySellStatus
        {
            get
            {
                var bidAskPricePoint = BidAskPricePoint;

                if (bidAskPricePoint >= 1D)
                {
                    return "STRONG_BUY";
                }
                else if (bidAskPricePoint >= .5D)
                {
                    return "LIKELY_BUY";
                }
                else if (bidAskPricePoint > 0D)
                {
                    return "WEAK_BUY";
                }
                else if (bidAskPricePoint == 0D)
                {
                    return "BIDASK_MID";
                }
                else if (bidAskPricePoint > -.5D)
                {
                    return "WEAK_SELL";
                }
                else if (bidAskPricePoint > -1D)
                {
                    return "LIKELY_SELL";
                }
                else if (bidAskPricePoint <= -1D)
                {
                    return "STRONG_SELL";
                }

                return "UNDETERMINED";
            }
        }
        public bool SingleLeg { get; set; }
        public long DayVolume { get; set; }

        public double UnderlyingPrice { get; set; }
        public double UnderlyingPriceNextDayOpen { get; set; }
        public double UnderlyingPriceClose { get; set; }
       
        public double QtyToVolume
        {
            get
            {
                return DayVolume == 0D ? 0D : (double)(Qty / DayVolume);
            }
        }

        public double QtyToOpenInterestChange
        {
            get
            {
                return OpenInterestChange == 0D ? 0D : (double)(Qty / OpenInterestChange);
            }
        }



        //BidAskSlippage = (OptionTimeSales[Price] - ((OptionTimeSales[Ask] + OptionTimeSales[Bid]) / 2)) / ((OptionTimeSales[Ask] - OptionTimeSales[Bid]) / 2)
    }
}
