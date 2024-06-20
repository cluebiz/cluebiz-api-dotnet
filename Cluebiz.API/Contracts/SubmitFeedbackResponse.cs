using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cluebiz.API.Contracts
{
    public class SubmitFeedbackResponse
    {

        [JsonProperty("ticketnumber")]
      public  string TicketNumber { get; set; }
    }
}
