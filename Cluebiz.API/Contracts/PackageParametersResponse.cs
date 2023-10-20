using Newtonsoft.Json;
using System;

namespace Cluebiz.API.Contracts
{
    public class PackageParametersResponse
    {
        [JsonProperty("softwarecatalogparameters")]
        public PackageParameter[] Parameters { get; set; }
    }

    public class PackageParameter
    {
        [JsonProperty("softwareCatalogParameterName")]
        public string Name { get; set; }

        [JsonProperty("softwareCatalogParameterId")]
        public Guid Id { get; set; }

        [JsonProperty("softwareCatalogParameterTitle")]
        public string Title { get; set; }

        [JsonProperty("fieldOrigin")]
        public string Origin { get; set; }

        [JsonProperty("fieldType")]
        public string Type { get; set; }

        [JsonProperty("fieldDescription")]
        public string Description { get; set; }

        [JsonProperty("fieldPattern")]
        public string Pattern { get; set; }

        [JsonProperty("fieldValue")]
        public string FieldValue { get; set; }

        [JsonProperty("fieldDefaultValue")]
        public string DefaultValue { get; set; }
    }
}