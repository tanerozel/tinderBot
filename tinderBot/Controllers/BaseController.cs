using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace tinderBot.Controllers
{
    public class BaseController : Controller
    {
        public static string TinderToken { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var requestHeader = context.HttpContext.Request.Headers;


            if (!requestHeader.TryGetValue("x-auth-token", out var XAuthToken))
            {
                context.Result = new BadRequestObjectResult("X-Auth-Token cannot be null.");
                return;
            }

            TinderToken = XAuthToken; 

        }
    }
}