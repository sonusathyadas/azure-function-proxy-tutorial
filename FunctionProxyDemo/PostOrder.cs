using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using FunctionProxyDemo.Models;
using Microsoft.Azure.ServiceBus;
using System.Text;

namespace FunctionProxyDemo
{
    public static class PostOrder
    {
        [FunctionName("PostOrder")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function,"post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var data = new StreamReader(req.Body).ReadToEnd();
            var order = JsonConvert.DeserializeObject<Order>(data);

            var connectionString = Environment.GetEnvironmentVariable("servicebus_connection");            
            var queue = "orders";
            IQueueClient queueClient = new QueueClient(connectionString, queue);
            Message message = new Message(Encoding.UTF8.GetBytes(data));
            await queueClient.SendAsync(message);

            log.LogInformation($"Order placed for customer {order.CustomerName} with order id {order.Id}");
            return new CreatedResult("", $"Order placed for customer {order.CustomerName} with order id {order.Id}");
        }
    }
}
