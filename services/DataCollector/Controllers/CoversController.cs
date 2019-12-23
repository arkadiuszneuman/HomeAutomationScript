using DataCollector.RabbitMq;
using DataCollector.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.Common.Models;
using System;

namespace DataCollector.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoversController : ControllerBase
    {
        private readonly ILogger<CoversController> logger;
        private readonly RabbitManager rabbitManager;

        public CoversController(
           ILogger<CoversController> logger,
           RabbitManager rabbitManager)
        {
            this.logger = logger;
            this.rabbitManager = rabbitManager;
        }

        [HttpGet("levelchanged")]
        public ActionResult<string> LevelChanged(int level)
        {
            new CoverLevelValidator().ValidateAndThrow(level);

            logger.LogInformation("Executed level changed");
            rabbitManager.PublishAsync(new CoverLevelChangedModel(level));

            return Ok($"Got level changed: {level}");
        }
    }
}
