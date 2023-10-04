using Newtonsoft.Json;
using System;

namespace Labtagon.Cloud.Packages.CluebizClient.Contracts
{
    public class IndividualSoftware
    {
        [JsonProperty("softwareId")]
        public Guid SoftwareId { get; set; }

        [JsonProperty("softwareName")]
        public string SoftwareName { get; set; }

        [JsonProperty("softwareManufacturer")]
        public string SoftwareManufacturer { get; set; }

        [JsonProperty("softwareProduct")]
        public string SoftwareProduct { get; set; }

        [JsonProperty("softwareVersion")]
        public string SoftwareVersion { get; set; }

        [JsonProperty("softwareRelease")]
        public string SoftwareRelease { get; set; }
    }

    public class GetSoftwareResponse
    {
        [JsonProperty("softwares")]
        public IndividualSoftware[] Softwares { get; set; }
    }
}
