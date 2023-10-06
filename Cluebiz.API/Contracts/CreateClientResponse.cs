using Newtonsoft.Json;
using System;

namespace Cluebiz.API.Contracts
{
    public class CreateClientResponse
    {
        [JsonProperty("clientId")]
        public Guid? Id { get; set; }
    }
}
