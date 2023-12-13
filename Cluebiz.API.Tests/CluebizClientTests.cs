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
                throw new Exception($"\nCustomer client '{CUSTOMER_CLIENT_NAME}' set up in appsettings.Local.json was not found.\nSee readme.txt file for test project setup.\n" +
                    $"Did you mean '{string.Join("', '", clients.Clients.Select(x => x.ClientName).Where(x => x.Contains(CUSTOMER_CLIENT_NAME.Substring(0, CUSTOMER_CLIENT_NAME.IndexOf(' ')))))}'");
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
                Debug.WriteLine($"Name: '{parameter.Name}' Id:'{parameter.Id}' Value: '{parameter.Value}' DefaultValue: '{parameter.DefaultValue}' ");
            }
        }

        [TestMethod]
        public async Task Should_GetGuidelines()
        {
            GuidelinesResponse response = await client.GetGuidelines(testClient.Id);



        }

        [TestMethod]
        public async Task Should_CreateAndDeleteGuideline()
        {

            Guid glId=  await client.CreateGuideline(testClient.Id,"TES TEST TEST TEST");

            GuidelinesResponse response = await client.GetGuidelines(testClient.Id);

            Guideline newGuideline =  response.Guidelines.FirstOrDefault(x => x.GuidelineID == glId);

            if (newGuideline == null) Assert.Fail("new guideline not found");

            await client.DeleteGuideline(testClient.Id,glId);

            GuidelinesResponse responseAfterDelete = await client.GetGuidelines(testClient.Id);

            newGuideline = responseAfterDelete.Guidelines.FirstOrDefault(x => x.GuidelineID == glId);

            if (newGuideline != null) Assert.Fail("new guideline not deleted");

        }


        //[TestMethod]
        //public async Task Should_GetCatalogItemParameters()
        //{
        //    // Adobe After Effects
        //    Guid catalogItemId = Guid.Parse("6fcab492-3075-4335-9825-4a3c70409995");

        //    PackageParametersResponse response = await client.GetSoftwareCatalogParameters(testClient.Id, catalogItemId);
        //    Assert.IsNotNull(response.Parameters);

        //    if(response.Parameters.Length == 0)
        //    {
        //        Assert.Inconclusive("Catalog Item has no parameters.");
        //    }


        //    foreach (var parameter in response.Parameters)
        //    {
        //        Debug.WriteLine($"Name: '{parameter.Name}' Id:'{parameter.Id}' Value: '{parameter.FieldValue}' DefaultValue: '{parameter.DefaultValue}' ");
        //    }
        //}


        //[TestMethod]
        //public async Task Should_SetCatalogItemParameters()
        //{
        //    // Chrome
        //    Guid catalogItemId = Guid.Parse("6fcab492-3075-4335-9825-4a3c70409995");

        //    PackageParametersResponse response = await client.GetSoftwareCatalogParameters(testClient.Id, catalogItemId);
        //    Assert.IsNotNull(response.Parameters);

        //    if (response.Parameters.Length == 0)
        //    {
        //        Assert.Inconclusive("Catalog Item has no parameters.");
        //    }

        //    PackageParameter homePageParameter = response.Parameters.FirstOrDefault(p => p.Name == "HomePage");

        //    if(homePageParameter == null)
        //    {
        //        Assert.Inconclusive("Homepage Parameter not found.");
        //    }

        //    string testValue = "www.mytesturl.com";
        //    string oldHomepage = homePageParameter.FieldValue;

        //    SetPackageParameterResponse setResponse = await client.SetSoftwareCatalogParameter(testClient.Id,catalogItemId,Guid.Parse(homePageParameter.Id), testValue);

        //    PackageParametersResponse newValueGetResponse = await client.GetSoftwareCatalogParameters(testClient.Id, catalogItemId);

        //    homePageParameter = newValueGetResponse.Parameters.FirstOrDefault(p => p.Name == "HomePage");

        //    Assert.IsTrue(testValue == homePageParameter.FieldValue,"Did not set the value of the parameter.");

        //    SetPackageParameterResponse setBackResponse = await client.SetSoftwareCatalogParameter(testClient.Id, catalogItemId, Guid.Parse(homePageParameter.Id), oldHomepage);



        //    //foreach (var parameter in response.Parameters)
        //    //{
        //    //    Debug.WriteLine($"Name: '{parameter.Name}' Id:'{parameter.Id}' FieldValue: '{parameter.FieldValue}' DefaultValue: '{parameter.DefaultValue}' ");
        //    //}
        //}
    }
}