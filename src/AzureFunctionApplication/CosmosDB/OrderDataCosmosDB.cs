using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AzureFunctionApplication.CosmosDB
{
    public class OrderDataCosmosDB
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
        public string BuyerId { get; set; }
        public decimal FinalPrice { get; set; }
        public string Address { get; set; }
        public IEnumerable<BasketItem> Items { get; set; }

        public class BasketItem 
        {
            public decimal UnitPrice { get; set; }
            public int Quantity { get; set; }
            public int CatalogItemId { get; set; }
            public int BasketId { get; set; }
        }
    }
}
