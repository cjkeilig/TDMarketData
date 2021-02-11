using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TDMarketData.BackTestingApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BackTestController : ControllerBase
    {

        private readonly ILogger<BackTestController> _logger;

        public BackTestController(ILogger<BackTestController> logger)
        {
            _logger = logger;
        }

        //[HttpGet]
        //public IEnumerable<BackTest> Get()
        //{

        //}
    }
}
