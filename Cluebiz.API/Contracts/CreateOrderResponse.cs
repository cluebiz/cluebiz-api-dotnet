using Newtonsoft.Json;
using System;

namespace Cluebiz.API.Contracts
{
    public class CreateOrderResponse
    {

        /// <summary>
        /// If the Order Id is null, this means the order could not be created, for example because the user
        /// already has an identical order.
        /// </summary>
        [JsonProperty("orderId")]
        public Guid? Id { get; set; }
    }
}
