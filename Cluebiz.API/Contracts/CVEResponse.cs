using Newtonsoft.Json;
using System;

namespace Labtagon.Cloud.Packages.CluebizClient.Contracts
{
    public class CVEResponse
    {
        [JsonProperty("softwarecatalogcves")]
        public CVE[] CVEs { get; set; }
    }

    public class CVE
    {
        [JsonProperty("softwareCatalogId")]
        public Guid? CatalogItem { get; set; }

        [JsonProperty("softwareCatalogMaxVersion")]
        public string MaxVersion { get; set; }

        [JsonProperty("softwareCatalogCVE")]
        public string CVEId { get; set; }

        [JsonProperty("softwareCatalogCVEUrl")]
        public string Url { get; set; }

        [JsonProperty("softwareCatalogImpact")]
        public string Impact { get; set; }

        [JsonProperty("softwareCatalogPublishedDate")]
        public DateTime PublishedDate { get; set; }

    }
}
