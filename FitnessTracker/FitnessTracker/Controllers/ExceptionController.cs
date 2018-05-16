using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracker.Controllers
{
    [Produces("application/json")]
    [Route("api/exception")]
    public class ExceptionController : Controller
    {
        [HttpGet]
        [Route("GetDate_RandomException")]
        public async Task<IActionResult> GetDateWithRandomExceptions()
        {
            var now = DateTime.Now;
            if (now.Second % 3 == 0)
            {
                throw new ArgumentException("Meaningless exception for the sake of the demo");
            }

            return Ok(DateTime.Now);
        }
    }
}