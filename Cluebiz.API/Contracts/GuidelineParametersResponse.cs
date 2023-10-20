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
        public string Id { get; set; }

        [JsonProperty("guidelineParameterName")]
        public string Name { get; set; }

        [JsonProperty("guidelineParameterTitle")]
        public string Title { get; set; }

        [JsonProperty("guidelineParameterValue")]
        public string Value { get; set; }
        
        [JsonProperty("guidelineParameterDescription")]
        public string Description { get; set; }

        [JsonProperty("guidelineParameterOrigin")]
        public string Origin { get; set; }

        [JsonProperty("guidelineParameterType")]
        public string Type { get; set; }

        [JsonProperty("guidelineParameterPattern")]
        public string Pattern { get; set; }

        [JsonProperty("guidelineParameterDefaultValue")]
        public string DefaultValue { get; set; }

        [JsonProperty("guidelineParameterRequired")]
        public string Required { get; set; }
    }
}
