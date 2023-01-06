using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PickMeFoodDelivery.Models.Models
{
    public class Restaurant
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "menu")]
        public List<FoodItem> FoodItems { get; set; }
    }
}
