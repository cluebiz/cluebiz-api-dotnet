using Cluebiz.API.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace Cluebiz.API.Tests
{

    [TestClass]
    public class CluebizClientTests
    {
        private ICluebizClient client; 
        private Client testClient;
        private IConfigurationRoot configuration;

        private string CUSTOMER_CLIENT_NAME;
        private string GUIDELINE_TITLE;

        [TestInitialize]
        public async Task Init()
        {
            try
            {
                var builder = new ConfigurationBuilder()
    .AddUserSecrets<CluebizClientTests>()
    .AddJsonFile("appsettings.Local.json", optional: false);

                configuration = builder.Build();
            }
            catch(Exception ee)
            {
                throw new Exception($"The test project setup is not complete. See readme.txt for instructions on how to set up the test project. {ee.Message}");
            }

            
            if(configuration["ServerAddress"] == null)
            {
                throw new Exception("Please make sure you've created a secret containing the keys `ServerAddress`, `UserId` and `Key`." +
                    "\n See readme.txt for instructions on how to set up the test project.");
            }

            client = CluebizClientFactory.GetClient(
                configuration["ServerAddress"],
                configuration["UserId"],
                configuration["Key"]
                );

            CUSTOMER_CLIENT_NAME = configuration["CustomerName"];
            GUIDELINE_TITLE = configuration["GuidelineTitle"];

            testClient = await GetTestClient();
        }

        private async Task<Client> GetTestClient()
        {
            ClientResponse clients = await client.GetClients();

            if (clients == null || clients.Clients == null || clients.Clients.Length == 0)
            {
                Assert.Inconclusive("There were no clients.");
            }
            Client c = clients.Clients.FirstOrDefault(x => x.ClientName == CUSTOMER_CLIENT_NAME);
            
            if(c == null)
            {
                throw new Exception($"Customer client '{CUSTOMER_CLIENT_NAME}' set up in appsettings.Local.json was not found. See readme.txt file for test project setup.");
            }

            return c;

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
            GuidelineParametersResponse guidelineParameters = await client.GetGuidelineParameters(testClient.Id,response.Guidelines.First(g => g.GuidelineTitle == GUIDELINE_TITLE).GuidelineID);
            Assert.IsNotNull(guidelineParameters);

            foreach (var parameter in guidelineParameters.Parameters)
            {
                Debug.WriteLine($"Name: '{parameter.Name}' Id:'{parameter.Id}'");
            }
        }
    }
}