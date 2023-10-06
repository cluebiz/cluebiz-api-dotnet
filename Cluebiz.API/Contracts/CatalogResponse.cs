using Newtonsoft.Json;
using System;

namespace Labtagon.Cloud.Packages.CluebizClient.Contracts
{
    public class CatalogResponse
    {
        [JsonProperty("softwarecatalogs")]
        public SoftwareCatalog[] SoftwareCatalog { get; set; }


    }
    public class SoftwareCatalog
    {
        [JsonProperty("softwareCatalogId")]
        public Guid CatalogId { get; set; }

        [JsonProperty("softwareCatalogName")]
        public string CatalogName { get; set; }

        [JsonProperty("softwareCatalogManufacturer")]
        public string Manufacturer { get; set; }

        [JsonProperty("softwareCatalogProduct")]
        public string Product { get; set; }

        [JsonProperty("softwareCatalogVersion")]
        public string Version { get; set; }

        [JsonProperty("softwareCatalogLogoURL")]
        public string Logo { get; set; }

        [JsonProperty("softwareCatalogDescriptionDE")]
        public string DescriptionDE { get; set; }

        [JsonProperty("softwareCatalogDescriptionEN")]
        public string DescriptionEN { get; set; }

        [JsonProperty("softwareCatalogDescriptionES")]
        public string DescriptionES { get; set; }

        [JsonProperty("softwareCatalogDescriptionFR")]
        public string DescriptionFR { get; set; }

        [JsonProperty("softwareCatalogDescriptionIT")]
        public string DescriptionIT { get; set; }
    }
}
