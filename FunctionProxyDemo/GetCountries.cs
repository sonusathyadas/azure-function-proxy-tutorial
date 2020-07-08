using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using FunctionProxyDemo.Services;

namespace FunctionProxyDemo
{
    public static class GetCountries
    {
        
        [FunctionName("GetCountries")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Getting the list of countries");
            var countries = DataService.GetCountries();
            return new OkObjectResult(countries);
        }
    }
}
