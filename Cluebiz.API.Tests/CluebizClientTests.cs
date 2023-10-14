using Cluebiz.API.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace Cluebiz.API.Tests
{

    [TestClass]
    public class CluebizClientTests
    {
        private CluebizClient client; 
        private Client testClient;
        private IConfigurationRoot configuration;

        [TestInitialize]
        public async Task Init()
        {
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<CluebizClientTests>();

            configuration = builder.Build();
            
            if(configuration["ServerAddress"] == null)
            {
                throw new Exception("Please make sure you've created a secret containing the keys `ServerAddress`, `UserId` and `Key`");
            }

            client = new CluebizClient(
                configuration["ServerAddress"],
                configuration["UserId"],
                configuration["Key"]
                );

            testClient = await GetTestClient();
        }

        private async Task<Client> GetTestClient()
        {
            ClientResponse clients = await client.GetClients();

            if (clients == null || clients.Clients == null || clients.Clients.Length == 0)
            {
                Assert.Inconclusive("There were no clients.");
            }
            return clients.Clients.FirstOrDefault(x => x.ClientName.StartsWith("Labtagon GmbH - Intern"));
        }

        [TestMethod]
        public async Task Should_GetSoftwareCatalogs()
        {
            CatalogResponse response = await client.GetSoftwareCatalog(testClient.Id);

            if (response.SoftwareCatalog.Length == 0)
            {
                Assert.Inconclusive("The request was successful, but there were no software catalogs in the response. Make sure to have software catalogs.");
            }

            Assert.IsNotNull(response);

            Debug.WriteLine($"Software Catalogs for client '{testClient.ClientName}' :");

            foreach (var catalog in response.SoftwareCatalog)
            {
                Debug.WriteLine($"Name: '{catalog.CatalogName}' Id:'{catalog.CatalogId}'");
                Debug.WriteLine($"   - Product: '{catalog.Product}'");
                Debug.WriteLine($"   - Manufacturer: '{catalog.Manufacturer}'");
            }
        }


        [TestMethod]
        public async Task Should_GetGuidelineParameters()
        {
            GuidelinesResponse response = await client.GetGuidelines(testClient.Id);
            GuidelineParametersResponse guidelineParameters = await client.GetGuidelineParameters(testClient.Id,response.Guidelines.First(g => g.GuidelineTitle == "Matrix42").GuidelineID);
            Assert.IsNotNull(guidelineParameters);

            
        }
    }
}