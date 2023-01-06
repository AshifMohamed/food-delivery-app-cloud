using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PickMeFoodDelivery.Models.Models;
using System.Collections.Generic;
using System.Threading;

namespace PickMeFoodDelivery.Orders
{
    public static class OrderFunction
    {
        private const string DbName = "PickMeFoodDeliveryDB";

        private const string ContainerName = "Orders";

        private const string QueueName = "foodorderqueue";

        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }

        //[FunctionName("PlaceOrder")]
        //public static ActionResult PlaceOrder([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "orders")]
        //HttpRequest req,
        //    [CosmosDB(
        //        databaseName:DbName,
        //        collectionName:ContainerName,
        //        ConnectionStringSetting = "COSMOSDB")]out dynamic document)
        //{
        //    string requestBody = new StreamReader(req.Body).ReadToEnd();
        //    Order orderC = JsonConvert.DeserializeObject<Order>(requestBody);
        //    document = orderC;

        //    return new OkObjectResult(orderC);
        //}

        [FunctionName("PlaceOrder")]
        public static ActionResult PlaceOrder([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "orders")]
        HttpRequest req,
           [ServiceBus(QueueName, Connection = "PickMeOrderQueue")] out Order queueMessage)
        {
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            Order newOrder = JsonConvert.DeserializeObject<Order>(requestBody);

            queueMessage = newOrder;

            return new OkObjectResult("Order placed successfully"); 
        }

        [FunctionName("AcceptOrder")]
        public static void AcceptOrder([CosmosDB(
                databaseName:DbName,
                collectionName:ContainerName,
                ConnectionStringSetting = "COSMOSDB")]out dynamic document,
            [ServiceBusTrigger(QueueName, Connection = "PickMeOrderQueue")] Order orderItem, ILogger log)
        {
            Thread.Sleep(20000);
            orderItem.OrderStatus = OrderStatus.ACCEPTED;
            document = orderItem;
        }

        [FunctionName("GetOrderStatus")]
        public static IActionResult GetOrderStatus([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "orders/{orderId}/status")] HttpRequest req,
             [CosmosDB(
                databaseName:DbName,
                collectionName:ContainerName,
                ConnectionStringSetting = "COSMOSDB",
                Id = "{orderId}",
                PartitionKey = "{orderId}")] Order order, ILogger log)
        {

            if (order != null)
            {
                return new OkObjectResult(order.OrderStatus.ToString());
            }
            else
            {
                var result = new ObjectResult($"Order {order.Id} not found");
                result.StatusCode = StatusCodes.Status404NotFound;
                return result;
            }
        }

        [FunctionName("GetOrders")]
        public static IActionResult GetOrders(
            [HttpTrigger(AuthorizationLevel.System, "get", Route = "orders")] HttpRequest req,
             [CosmosDB(
                databaseName: DbName,
                collectionName: ContainerName,
                ConnectionStringSetting = "COSMOSDB",
                SqlQuery = "SELECT * FROM c")]
                IEnumerable<Order> orders)
        {

            return new OkObjectResult(orders);
        }
    }
}
