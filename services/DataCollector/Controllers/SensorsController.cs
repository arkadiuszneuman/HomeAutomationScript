using DataCollector.Config;
using DataCollector.RabbitMq;
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
        private readonly ILogger<SensorsController> _logger;
        private readonly RabbitManager _rabbitManager;

        public SensorsController(
            ILogger<SensorsController> logger,
            RabbitManager rabbitManager)
        {
            _logger = logger;
            _rabbitManager = rabbitManager;
        }

        [HttpGet("StairsSensorDown")]
        public ActionResult<string> StairsSensorDown()
        {
            Task.Factory.StartNew(async () =>
            {
                _logger.LogInformation("Executed stairs down sensor");
                await _rabbitManager.PublishAsync(new TriggeredBottomStairSensorModel { DateTime = DateTime.Now });
            });

            return Ok("Got sensor down info");
        }

        [HttpGet("StairsSensorUp")]
        public ActionResult<string> StairsSensorUp()
        {
            Task.Factory.StartNew(async () =>
            {
                _logger.LogInformation("Executed stairs up sensor");
                await _rabbitManager.PublishAsync(new TriggeredUpperStairSensorModel { DateTime = DateTime.Now });
            });
            
            return Ok("Got sensor up info");
        }
    }
}