using Newtonsoft.Json;
using System;

namespace Cluebiz.API.Contracts
{
    public class RobotSessionTokensResponse
    {
        [JsonProperty("tokens")]
        public GenericToken[] Tokens { get; set; }
    }


    public class GenericToken
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("tokenType")]
        public string TokenType { get; set; }

        [JsonProperty("guidelineId")]
        public Guid? GuidelineId { get; set; }

        [JsonProperty("endDate")]
        public DateTime EndDate { get; set; }
    }
}
