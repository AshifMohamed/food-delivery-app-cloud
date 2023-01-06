using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PickMeFoodDelivery.Models.Models
{
    public class Order
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "customerId")]
        public string CustomerId { get; set; }

        [JsonProperty(PropertyName = "restaurantId")]
        public string RestaurantId { get; set; }

        [JsonProperty(PropertyName = "orderItems")]
        public List<OrderQuantity> OrderItems { get; set; }

        [JsonProperty(PropertyName = "orderStatus")]
        public OrderStatus OrderStatus { get; set; }
    }

    public class OrderQuantity
    {
        [JsonProperty(PropertyName = "itemId")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "quantity")]
        public int Quantity { get; set; }
    }
    public enum OrderStatus
    {
        NEW,
        ACCEPTED,
        DELIVERED,
        CANCELLED
    }
}
