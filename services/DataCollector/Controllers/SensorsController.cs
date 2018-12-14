using DataCollector.Config;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IBusClient busClient;

        public SensorsController(RabbitMqConfig rabbitMqConfig)
        {
            var busConfig = new RawRabbitConfiguration
            {
                Port = rabbitMqConfig.Port,
                Hostnames = { rabbitMqConfig.Hostname }
            };
            busClient = BusClientFactory.CreateDefault(busConfig);
        }

        [HttpGet("StairsSensorDown")]
        public async Task<ActionResult<string>> StairsSensorDown()
        {
            await busClient.PublishAsync(new TriggeredBottomStairSensorModel { DateTime = DateTime.Now });
            return Ok("Got sensor down info");
        }

        [HttpGet("StairsSensorUp")]
        public async Task<ActionResult<string>> StairsSensorUp()
        {
            await busClient.PublishAsync(new TriggeredUpperStairSensorModel { DateTime = DateTime.Now });
            return Ok("Got sensor up info");
        }

        [HttpGet("Test")]
        public async Task<ActionResult<string>> Test()
        {
            await busClient.PublishAsync(new TestModel { Message = "asd" });
            return Ok("Test");
        }
    }
}