using Newtonsoft.Json;
using System;

namespace Labtagon.Cloud.Packages.CluebizClient.Contracts
{
    public class StartBakingProcessResponse
    {
        [JsonProperty("processId")]
        public Guid ProcessId { get; set; }

    }

    public class GetBakingProcessesResponse
    {
        [JsonProperty("processes")]
        public CluebizBakingProcess[] Processes { get; set; } 
    }

    public class CluebizBakingProcess
    {

        [JsonProperty("processId")]
        public Guid ProcessId { get; set; }

        [JsonProperty("softwareCatalogDeployId")]
        public Guid? SoftwareCatalogDeployId { get; set; }

        [JsonProperty("guidelineId")]
        public Guid GuidelineId { get; set; }

        [JsonProperty("softwareDeployId")]
        public Guid? SoftwareDeployId { get; set; }

        [JsonProperty("percentfinished")]
        public string PercentFinished { get; set; }

        [JsonProperty("downloadlink")]
        public string DownloadLink { get; set; }

        [JsonProperty("starttime")]
        public DateTime StartTime { get; set; }


    }

   
}
