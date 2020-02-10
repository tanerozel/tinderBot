using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using Microsoft.Extensions.Caching.Memory;

namespace tinderBot.Controllers
{

    [ApiController]
    [Route("api/bot")]
    public class TinderBotController : BaseController
    {

        private IMemoryCache _memoryCache;

        public TinderBotController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;

        }

        [Route("run"), HttpGet]
        public async Task<IActionResult> Run()
        {

            var tinderClient = new TinderBot(TinderToken, _memoryCache);

            var data = await tinderClient.GetTindePersone();

            return Ok(data);


        }

        [Route("test"), HttpGet]
        public async Task<IActionResult> Test()
        {

            var tinderClient = new TinderBot("asdf", _memoryCache);
            var data =   await  tinderClient.test();

            return Ok(data);


        }


    }



}
