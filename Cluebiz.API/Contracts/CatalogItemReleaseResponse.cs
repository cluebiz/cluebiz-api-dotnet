﻿using Newtonsoft.Json;

namespace Cluebiz.API.Contracts
{
    public class CatalogItemReleaseResponse
    {
        [JsonProperty("softwarecatalogreleases")]
        public SoftwareCatalogRelease[] Catalogs { get; set; }
    }

    public class SoftwareCatalogRelease
    {
        [JsonProperty("softwareCatalogId")]
        public string PackageId { get; set; }

        [JsonProperty("softwareCatalogDeployId")]
        public string DeployId { get; set; }

        [JsonProperty("softwareCatalogReleaseName")]
        public string ReleaseName { get; set; }

        [JsonProperty("softwareCatalogPackageType")]
        public string PackageType { get; set; }

        [JsonProperty("softwareCatalogArchitecture")]
        public string Architecture { get; set; }

        [JsonProperty("softwareCatalogSize")]
        public string Size { get; set; }

        [JsonProperty("softwareCatalogArchive")]
        public bool Archive { get; set; }

        [JsonProperty("softwareCatalogLanguages")]
        public string Languages { get; set; }
    }
}
