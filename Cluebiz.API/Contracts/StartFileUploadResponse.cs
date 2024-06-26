using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cluebiz.API.Contracts
{
    public class StartFileUploadResponse
    {
        [JsonProperty("fileId")]
        public Guid FileId { get; set; }

    }
}
