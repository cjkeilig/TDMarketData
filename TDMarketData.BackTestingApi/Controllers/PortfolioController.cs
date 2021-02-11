using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TDMarketData.BackTesting.Data;
using TDMarketData.BackTesting.Data.Models;
using System.Linq;

namespace TDMarketData.BackTestingApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PortfolioController : ControllerBase
    {
        private readonly BackTestContext _context;

        public PortfolioController(BackTestContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult GetPortfolio(int portfolioId)
        {
            var portfolio = _context.Portfolio.Include(p => p.Positions).First(p => p.Id == portfolioId);

            var candles = _context.Set<OptionCandle>().FromSqlInterpolated($"SELECT oc.*, p.Id AS PositionId FROM Positions p JOIN OptionCandles oc ON p.OptionContractId = oc.OptionContractId WHERE p.PortfolioId = {portfolioId}").ToList();

            portfolio.Positions.ToList().ForEach(p => p.OptionCandles = candles.Where(c => c.OptionContractId == p.OptionContractId).ToList());

            return new JsonResult(portfolio);
        }
    }
}
