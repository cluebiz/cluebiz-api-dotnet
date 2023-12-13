using Newtonsoft.Json;
using System;

namespace Cluebiz.API.Contracts
{
    public class GuidelinesResponse
    {
        [JsonProperty("guidelines")]
        public Guideline[] Guidelines { get; set; }
    }

    public class Guideline
    {
        [JsonProperty("guidelineId")]
        public Guid GuidelineID { get; set; }

        [JsonProperty("guidelineTitle")]
        public string GuidelineTitle { get; set; }
    }
}
