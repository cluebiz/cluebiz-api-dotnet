using Newtonsoft.Json;
using System;

namespace Cluebiz.API.Contracts
{
    public class CreateGuidelineResponse
    {
        [JsonProperty("guidelineId")]
        public Guid GuidelineId { get; set; }
    }
}
