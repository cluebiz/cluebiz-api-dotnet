using Newtonsoft.Json;
using System;

namespace Cluebiz.API.Contracts
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
