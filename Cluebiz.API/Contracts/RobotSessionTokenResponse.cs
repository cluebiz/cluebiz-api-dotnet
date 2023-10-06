using Newtonsoft.Json;

namespace Labtagon.Cloud.Packages.CluebizClient.Contracts
{
    public class RobotSessionTokenResponse
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
