using Newtonsoft.Json;

namespace Cluebiz.API.Contracts
{
    public class GuidelineParametersResponse
    {
        [JsonProperty("guidelineParameters")]
        public GuidelineParameter[] Parameters { get; set; }
    }

    public class GuidelineParameter
    {
        [JsonProperty("guidelineParameterId")]
        public string GuidelineParameterId { get; set; }

        [JsonProperty("guidelineParameterName")]
        public string GuidelineParameterName { get; set; }

        [JsonProperty("guidelineParameterValue")]
        public string GuidelineParameterValue { get; set; }

        [JsonProperty("guidelineParameterType")]
        public string GuidelineParameterType { get; set; }

        [JsonProperty("guidelineParameterPattern")]
        public string GuidelineParameterPattern { get; set; }

        [JsonProperty("guidelineParameterDefaultValue")]
        public string GuidelineParameterDefaultValue { get; set; }

        [JsonProperty("guidelineParameterRequired")]
        public string Required { get; set; }
    }
}
