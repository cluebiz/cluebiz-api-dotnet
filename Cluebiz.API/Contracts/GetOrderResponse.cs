using Newtonsoft.Json;
using System;

namespace Labtagon.Cloud.Packages.CluebizClient.Contracts
{
    public class GetOrderResponse
    {
        [JsonProperty("orders")]
        public Order[] Orders { get; set; }
    }

    public class Order
    {
        [JsonProperty("orderId")]
        public Guid Id { get; set; }
        //"orderName": "",
        //    "orderStatus": "Preparation",
        //    "Manufacturer": "",
        //    "Product": "",
        //    "ReleaseName
        [JsonProperty("orderName")]
        public string Name { get; set; }

        [JsonProperty("Manufacturer")]
        public string Manufacturer { get; set; }

        [JsonProperty("Product")]
        public string ProductName { get; set; }

        [JsonProperty("ReleaseName")]
        public string ReleaseVersion { get; set; }

        [JsonProperty("orderStatus")]
        public string Status { get; set; }

        [JsonProperty("softwareId")]
        public Guid? SoftwareId { get; set; }
        
    }
}
