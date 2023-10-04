using Newtonsoft.Json;
using System;

namespace Labtagon.Cloud.Packages.CluebizClient.Contracts
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

