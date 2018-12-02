using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace datacollector.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorsController : ControllerBase
    {
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
    }
}