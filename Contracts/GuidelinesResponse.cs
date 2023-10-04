using Newtonsoft.Json;
using System;

namespace Labtagon.Cloud.Packages.CluebizClient.Contracts
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

        public GuidelineParameter[] Paramters { get;set; }
    }
}
