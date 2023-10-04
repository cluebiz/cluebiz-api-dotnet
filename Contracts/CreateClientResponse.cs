using Newtonsoft.Json;
using System;

namespace Labtagon.Cloud.Packages.CluebizClient.Contracts
{
    public class CreateClientResponse
    {
        [JsonProperty("clientId")]
        public Guid? Id { get; set; }
    }
}
