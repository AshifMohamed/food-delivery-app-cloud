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

namespace PickMeFoodDelivery.Restaurants
{
    public static class RestaurantFunction
    {
        private const string DbName = "PickMeFoodDeliveryDB";

        private const string ContainerName = "Restaurants";

        [FunctionName("SearchRestaurant")]
        public static IActionResult SearchRestaurant(
           [HttpTrigger(AuthorizationLevel.System, "get", Route = "restaurants/searchRestaurant/{restaurantName}")] HttpRequest req,
            [CosmosDB(
                databaseName:DbName,
                collectionName:ContainerName,
                ConnectionStringSetting = "COSMOSDB",
                SqlQuery = "SELECT * FROM c WHERE CONTAINS(c.name, {restaurantName})")]
                IEnumerable<Restaurant> restaurants)
        {
            return new OkObjectResult(restaurants);
        }

        [FunctionName("GetRestaurants")]
        public static IActionResult GetRestaurants(
            [HttpTrigger(AuthorizationLevel.System, "get", Route = "restaurants/getRestaurants")] HttpRequest req,
             [CosmosDB(
                databaseName: DbName,
                collectionName: ContainerName,
                ConnectionStringSetting = "COSMOSDB",
                SqlQuery = "SELECT * FROM c")]
                IEnumerable<Restaurant> restaurants)
        {

            return new OkObjectResult(restaurants);
        }
    }
}
