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

        [FunctionName("PlaceOrder")]
        public static ActionResult PlaceOrder([HttpTrigger(AuthorizationLevel.Function, "post", Route = "orders")]
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
        public static IActionResult GetOrderStatus([HttpTrigger(AuthorizationLevel.Function, "get", Route = "orders/{orderId}/status")] HttpRequest req,
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
                var result = new ObjectResult($"Order {order.Id} not found")
                {
                    StatusCode = StatusCodes.Status404NotFound
                };
                return result;
            }
        }

        [FunctionName("CancelOrder")]
        public static IActionResult CancelOrder([HttpTrigger(AuthorizationLevel.Function, "get", Route = "orders/{orderId}/cancel")] HttpRequest req,
             [CosmosDB(
                databaseName:DbName,
                collectionName:ContainerName,
                ConnectionStringSetting = "COSMOSDB",
                Id = "{orderId}",
                PartitionKey = "{orderId}")] Order order, ILogger log)
        {

            if (order != null)
            {
                order.OrderStatus = OrderStatus.CANCELLED;
                return new OkObjectResult($"Order {order.Id} Cancelled successfully");
            }
            else
            {
                var result = new ObjectResult($"Order {order.Id} not found")
                {
                    StatusCode = StatusCodes.Status404NotFound
                };
                return result;
            }
        }

        [FunctionName("GetOrders")]
        public static IActionResult GetOrders(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "orders")] HttpRequest req,
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
