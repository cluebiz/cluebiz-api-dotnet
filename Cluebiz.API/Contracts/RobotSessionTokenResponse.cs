using Newtonsoft.Json;

namespace Cluebiz.API.Contracts
{
    public class RobotSessionTokenResponse
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
