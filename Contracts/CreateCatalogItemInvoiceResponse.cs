using Newtonsoft.Json;
using System;

namespace Labtagon.Cloud.Packages.CluebizClient.Contracts
{
    public class CreateCatalogItemInvoiceResponse
    {
        [JsonProperty("response")]
        public string Response { get; set; }

        [JsonProperty("data")]
        public CreateCatalogItemInvoiceResponseData Data { get; set; }
    }

    public class CreateCatalogItemInvoiceResponseData
    {
        [JsonProperty("invoiceId")]
        public Guid InvoiceId { get; set; }
    }
}
