using System;
using System.Collections.Generic;
using System.Text;

namespace Cluebiz.API
{

    /// <summary>
    /// Factory class to generate CluebizClients.
    /// </summary>
    public static class CluebizClientFactory
    {

        /// <summary>
        /// Creates a new cluebiz client, given correct secrets.
        /// </summary>
        /// <param name="serverAddress">Endpoint of the cluebiz API.</param>
        /// <param name="userId">The cluebiz API user id.</param>
        /// <param name="key">Secret key, belonging to the userId.</param>
        public static ICluebizClient GetClient(string serverAddress, string userId, string key)
        {
            return new CluebizClient(serverAddress,userId,key);
        }
    }
}
