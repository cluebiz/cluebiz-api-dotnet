using Newtonsoft.Json;

namespace Labtagon.Cloud.Packages.CluebizClient.Contracts
{
    public class CatalogReleaseDownloadResponse
    {
        [JsonProperty("downloadlink")]
        public string Downloadlink { get; set; }
    }
}
