﻿using Microsoft.AspNetCore.Mvc;
using RawRabbit;
using RawRabbit.Configuration;
using RawRabbit.vNext;
using Services.Common.Models;
using System.Threading.Tasks;

namespace datacollector.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorsController : ControllerBase
    {
        private readonly IBusClient busClient;

        public SensorsController()
        {
            var busConfig = new RawRabbitConfiguration
            {
                Username = "guest",
                Password = "guest",
                Port = 5672,
                VirtualHost = "/",
                Hostnames = { "localhost" }
            };
            busClient = BusClientFactory.CreateDefault(busConfig);
        }

        [HttpGet("StairsSensorDown")]
        public ActionResult<string> StairsSensorDown()
        {
            return Ok("Got sensor down info");
        }

        [HttpGet("StairsSensorUp")]
        public ActionResult<string> StairsSensorUp()
        {
            return Ok("Got sensor up info");
        }


        [HttpGet("Test")]
        public async Task<ActionResult<string>> Test()
        {
            await busClient.PublishAsync<TestModel>(new TestModel { Message = "asd" });
            return Ok("Test");
        }
    }
}