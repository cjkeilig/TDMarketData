using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDMarketData.BackTesting.Data.Models;

namespace TDMarketData.BackTesting.Data
{
    public class PositionDbRepository
    {
        private readonly BackTestContext _backTestContext;

        public PositionDbRepository(BackTestContext backTestContext)
        {
            _backTestContext = backTestContext;
        }

        public async Task CreatePositionAsync(Position position)
        {
            await _backTestContext.AddAsync(position);

            await _backTestContext.SaveChangesAsync();
        }
    }
}
