using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PickMeFoodDelivery.Models.Models
{
    public class FoodItem
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "price")]
        public double Price { get; set; }
    }
}
