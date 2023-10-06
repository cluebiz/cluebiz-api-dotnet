using Newtonsoft.Json;
using System;

namespace Cluebiz.API.Contracts
{
    public class ClientResponse
    {

        [JsonProperty("clients")]
        public Client[] Clients { get; set; }

    }

    public class Client
    {
        [JsonProperty("clientId")]
        public Guid Id { get; set; }


        [JsonProperty("clientName")]
        public string ClientName { get; set; }
    }
}

