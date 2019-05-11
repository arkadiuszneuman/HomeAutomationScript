using DataCollector.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RawRabbit;
using RawRabbit.Configuration;
using RawRabbit.vNext;
using Services.Common.Models;
using System;
using System.Threading.Tasks;

namespace datacollector.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorsController : ControllerBase
    {
        private readonly IBusClient _busClient;
        private readonly ILogger<SensorsController> _logger;

        public SensorsController(IBusClient busClient,
            ILogger<SensorsController> logger)
        {
            this._busClient = busClient;
            _logger = logger;
        }

        [HttpGet("StairsSensorDown")]
        public async Task<ActionResult<string>> StairsSensorDown()
        {
            _logger.LogInformation("Executed stairs down sensor");
            await _busClient.PublishAsync(new TriggeredBottomStairSensorModel { DateTime = DateTime.Now });
            return Ok("Got sensor down info");
        }

        [HttpGet("StairsSensorUp")]
        public async Task<ActionResult<string>> StairsSensorUp()
        {
            _logger.LogInformation("Executed stairs up sensor");
            await _busClient.PublishAsync(new TriggeredUpperStairSensorModel { DateTime = DateTime.Now });
            return Ok("Got sensor up info");
        }
    }
}