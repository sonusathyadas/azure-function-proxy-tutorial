using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using FunctionProxyDemo.Services;

namespace FunctionProxyDemo
{
    public static class GetCountryByCode
    {
        [FunctionName("GetCountryByCode")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetCountryByCode/{code}")] HttpRequest req,
            string code,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var country=DataService.GetCountryByCode(code);
            if (country != null)
                return new OkObjectResult(country);
            else
                return new NotFoundResult();
        }
    }
}
