using Newtonsoft.Json;
using System;
using System.Linq;

namespace Labtagon.Cloud.Packages.CluebizClient.Contracts
{
    public class GetOrderDetailsResponse
    {
        [JsonProperty("orderId")]
        public Guid OrderId { get; set; }

        [JsonProperty("orderdetails")]
        public OrderField[] Fields { get; set; }

        public string Manufacturer => Fields.FirstOrDefault(f => f.Name == "Manufacturer")?.Value;

        public string ProductName => Fields.FirstOrDefault(f => f.Name == "Product")?.Value;

        public string Version => Fields.FirstOrDefault(f => f.Name == "Version")?.Value;

        //Should be id.
        public string Status => Fields.FirstOrDefault(f => f.Name == "Status")?.Value;

        public string AppOwner => Fields.FirstOrDefault(f => f.Name == "AppOwner")?.Value;

        public string PackageType => Fields.FirstOrDefault(f => f.Name == "PackageType")?.Value;

        public string OrderType => Fields.FirstOrDefault(f => f.Name == "OrderType")?.Value;

        public string OrderDuration => Fields.FirstOrDefault(f => f.Name == "OrderDuration")?.Value;

        public string WizardId => Fields.FirstOrDefault(f => f.Name == "WizardId")?.Value;
    }

    public class OrderField
    {
        [JsonProperty("orderFieldId")]
        public Guid Id { get; set; }

        [JsonProperty("orderFieldName")]
        public string Name { get;set; }

        [JsonProperty("orderFieldValue")]
        public string Value { get; set; }

        [JsonProperty("orderFieldType")]
        public string Type { get; set; }

        [JsonProperty("orderFieldPattern")]
        public string Pattern { get; set; }
    }
}
