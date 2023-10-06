using Newtonsoft.Json;
using System;

namespace Labtagon.Cloud.Packages.CluebizClient.Contracts
{
    public class CreateGuidelineResponse
    {
        [JsonProperty("guidelineId")]
        public Guid GuidelineId { get; set; }
    }
}
