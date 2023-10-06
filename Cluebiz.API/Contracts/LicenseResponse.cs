using Newtonsoft.Json;
using System;

namespace Labtagon.Cloud.Packages.CluebizClient.Contracts
{
    public class LicenseResponse
    {
        public License[] Licenses { get; set; } 
    }

    public class License
    {
        [JsonProperty("clientId")]
        public Guid ClientId { get; set; }

        /// <summary>
        /// Robot or ..?
        /// </summary>
        [JsonProperty("licenseType")]
        public string LicenseType { get; set; }


        /// <summary>
        /// The license Id is not unique, its a reference to the existing license types
        /// </summary>
        [JsonProperty("licenseId")]
        public Guid LicenseId { get; set; }

        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }
    }
}
