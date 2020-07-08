using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FunctionProxyDemo
{
    public static class GetCurrentTime
    {
        [FunctionName("GetCurrentTime")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous,"post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Requesting for the current server time");

            var time = DateTime.Now.ToShortTimeString();
            return new OkObjectResult(time);
        }
    }
}
