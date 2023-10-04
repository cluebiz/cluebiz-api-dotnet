using Newtonsoft.Json;
using System;

namespace Labtagon.Cloud.Packages.CluebizClient.Contracts
{
    public class GetCatalogItemInvoicesResponse
    {
        [JsonProperty("invoices")]
        public Invoice[] Invoices { get; set; }
    }

    public class Invoice
    {
        [JsonProperty("invoiceAmount", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Include)]
        public int? Amount { get; set; }

        [JsonProperty("invoiceCurrency", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Include)]
        public string Currency { get; set; }

        [JsonProperty("invoiceDate", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Include)]
        public DateTime? CreatedDate { get; set; }

        [JsonProperty("invoiceId", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Include)]
        public Guid Id { get; set; }

        [JsonProperty("invoiceStatus", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Include)]
        public string Status { get; set; }

        [JsonProperty("items", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Include)]
        public InvoiceItem[] Items { get; set; }
    }

    public class InvoiceItem
    {
        [JsonProperty("invoiceItemId", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Include)]
        public Guid Id { get; set; }

        [JsonProperty("invoiceItemPrice", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Include)]
        public int Price { get; set; }

        /// <summary>
        /// The id of the type of invoice. <see cref="InvoiceProduct"/>
        /// </summary>
        [JsonProperty("invoiceProductId", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Include)]
        public Guid ProductId { get; set; }

        [JsonProperty("invoiceProductTitle", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Include)]
        public string Title { get; set; }

        /// <summary>
        /// The id of the catalog item.
        /// </summary>
        [JsonProperty("softwareCatalogDeployId", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Include)]
        public Guid SoftwareCatalogDeployId { get; set; }
    }
}
