using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDMarketData.BackTesting.Data.Helpers
{
    public static class SqlHelpers
    {
        public static async Task<Dictionary<string, UnderlyingData>> GetCurrentUnderlying(BackTestContext context)
        {

            var connection = context.Database.GetDbConnection();
            long timestamp = 0;

            using (DbCommand cmd = connection.CreateCommand())
            {

                cmd.CommandText = "SELECT MAX(c.\"Datetime\") FROM public.\"OptionCandles\" c";


                if (connection.State.Equals(ConnectionState.Closed)) { connection.Open(); }

                timestamp = (long)await cmd.ExecuteScalarAsync();
            }

            var dateStr = timestamp.ToString();
            var date  = new DateTime(int.Parse(dateStr.Substring(0, 4)), int.Parse(dateStr.Substring(4, 2)), int.Parse(dateStr.Substring(6, 2)));

            var dateParam = date.ToString("yyyy-MM-dd mm:ss:ff");

            var underlyingPrices = context.Set<UnderlyingData>().FromSqlInterpolated($"SELECT UnderlyingSymbol, ds.\"iv\" AS Volatility, UnderlyingPrice, ds.\"Date\" FROM public.\"DailyStats\" ds, (SELECT MAX(oc.\"UnderlyingPrice\") AS UnderlyingPrice, oct.\"UnderlyingSymbol\" AS UnderlyingSymbol FROM public.\"OptionCandles\" oc JOIN public.\"OptionContracts\" oct ON oc.\"OptionContractId\" = oct.\"Id\" WHERE oc.\"Datetime\" = {timestamp} GROUP BY oct.\"UnderlyingSymbol\") symbolPrice WHERE ds.\"symbol\" = symbolPrice.UnderlyingSymbol");

            var prices = await underlyingPrices.ToListAsync();
            prices = prices.Where(p => p.Date == date).ToList();

            return prices.ToDictionary(u => u.UnderlyingSymbol, u => u);
        }

    }

    public class UnderlyingData
    {
        public string UnderlyingSymbol { get; set; }
        public double UnderlyingPrice { get; set; }
        public string Volatility { get; set; }
        public DateTime Date { get; set; }
    }
}
