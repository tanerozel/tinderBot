using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace tinderBot.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
    
        [HttpGet]
        public async Task<IActionResult> Get()
        {

            var tinderClient =  new TinderBot("https://api.gotinder.com/v2/", "e10c7685-c237-4416-9abf-a1278c802bec");

           var data =  await tinderClient.GetTindePersone();

            return data;


        }
    }
}
