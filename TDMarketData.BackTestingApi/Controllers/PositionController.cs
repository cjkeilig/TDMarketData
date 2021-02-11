using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDMarketData.BackTesting.Data;
using TDMarketData.BackTesting.Data.Models;

namespace TDMarketData.BackTestingApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PositionController : ControllerBase
    {
        private readonly PositionDbRepository _positionDbRepository;
        public PositionController(PositionDbRepository positionDbRepository)
        {
            _positionDbRepository = positionDbRepository;
        }

        [HttpPost]
        public async Task<ActionResult> SavePosition(Position position)
        {
            try
            {
                await _positionDbRepository.CreatePositionAsync(position);

                return Ok(position);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.StackTrace);
            }
        }
    }
}
