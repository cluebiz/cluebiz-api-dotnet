using Newtonsoft.Json;

namespace Cluebiz.API.Contracts
{
    public class CatalogReleaseDownloadResponse
    {
        [JsonProperty("downloadlink")]
        public string Downloadlink { get; set; }
    }
}
