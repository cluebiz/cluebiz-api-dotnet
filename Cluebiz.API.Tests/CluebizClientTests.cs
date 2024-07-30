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
            catch (Exception ee)
            {
                throw new Exception($"The test project setup is not complete. See readme.txt for instructions on how to set up the test project. {ee.Message}");
            }


            if (configuration["ServerAddress"] == null)
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

            if (c == null)
            {
                throw new Exception($"\nCustomer client '{CUSTOMER_CLIENT_NAME}' set up in appsettings.Local.json was not found.\nSee readme.txt file for test project setup.\n" +
                    $"Did you mean '{string.Join("', '", clients.Clients.Select(x => x.ClientName).Where(x => x.Contains(CUSTOMER_CLIENT_NAME.Substring(0, CUSTOMER_CLIENT_NAME.IndexOf(' ')))))}'");
            }

            return c;

        }


        [TestMethod]
        private async Task Should_CreateClient()
        {
            await client.CreateClient("TEMPORARY TEST CLIENT");

            ClientResponse clients = await client.GetClients();

            Client c = clients.Clients.First(x => x.ClientName == "TEMPORARY TEST CLIENT");

            if(c == null)
            {
                Assert.Fail("Client was not created");
            }

            await client.RemoveClient(c.Id);

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
            GuidelineParametersResponse guidelineParameters = await client.GetGuidelineParameters(testClient.Id, response.Guidelines.FirstOrDefault(g => g.GuidelineTitle == GUIDELINE_TITLE).GuidelineID);
            Assert.IsNotNull(guidelineParameters);

            foreach (var parameter in guidelineParameters.Parameters)
            {
                Debug.WriteLine($"Name: '{parameter.Name}' Id:'{parameter.Id}' Value: '{parameter.Value}' DefaultValue: '{parameter.DefaultValue}' ");
            }
        }


        [TestMethod]
        public async Task Should_SetGuidelineParameter()
        {
            GuidelinesResponse response = await client.GetGuidelines(testClient.Id);

            Guideline? guideline = response.Guidelines.FirstOrDefault(g => g.GuidelineTitle == GUIDELINE_TITLE);

            if (guideline == null)
            {
                Assert.Inconclusive("No guideline found.");
            }

            GuidelineParametersResponse guidelineParameters = await client.GetGuidelineParameters(testClient.Id, guideline.GuidelineID);

            GuidelineParameter? param = guidelineParameters.Parameters.FirstOrDefault(p => p.Id == "95b86fa5-7062-478e-b2c9-e6954ac7ddb8");

            if (param == null)
            {
                Assert.Inconclusive("No parameter found");
            }

            if (param.Value == "500")
            {
                Assert.Inconclusive("Parameter already has value to be set.");
            }


            string originalValue = param.Value;

            await client.SetGuidelineParameter(testClient.Id, guideline.GuidelineID, Guid.Parse(param.Id), "500");

            guidelineParameters = await client.GetGuidelineParameters(testClient.Id, guideline.GuidelineID);

            param = guidelineParameters.Parameters.FirstOrDefault(p => p.Id == "95b86fa5-7062-478e-b2c9-e6954ac7ddb8");

            if (param == null)
            {
                Assert.Fail("Parameter does not exist after setting it. This should not be possible.");
            }

            if (param.Value != "500")
            {
                Assert.Inconclusive("Parameter was not set to the new value.");
            }

            await client.SetGuidelineParameter(testClient.Id, guideline.GuidelineID, Guid.Parse("95b86fa5-7062-478e-b2c9-e6954ac7ddb8"), originalValue);


        }

        [TestMethod]
        public async Task Should_GetGuidelines()
        {

            GuidelinesResponse response = await client.GetGuidelines(Guid.Parse("11027d41-d286-465e-a15f-18b802f3b173"));

            GuidelinesResponse response2 = await client.GetGuidelines(testClient.Id);



        }

        [TestMethod]
        public async Task Should_CreateAndDeleteGuideline()
        {

            Guid glId = await client.CreateGuideline(testClient.Id, "TES TEST TEST TEST");

            GuidelineParametersResponse paramR = await client.GetGuidelineParameters(testClient.Id, glId);

            await client.SetGuidelineParameter(testClient.Id, glId, Guid.Parse("bb82ace7-c500-e717-3dfd-51033f5a09b8"), "opsi");



            GuidelinesResponse response = await client.GetGuidelines(testClient.Id);

            Guideline newGuideline = response.Guidelines.FirstOrDefault(x => x.GuidelineID == glId);

            if (newGuideline == null) Assert.Fail("new guideline not found");

            var paramsResponse = await client.GetGuidelineParameters(testClient.Id, glId);

            var deployTypeParam = paramsResponse.Parameters.FirstOrDefault(p => p.Id == "bb82ace7-c500-e717-3dfd-51033f5a09b8");


            await client.SetGuidelineParameter(testClient.Id, newGuideline.GuidelineID, Guid.Parse("bb82ace7-c500-e717-3dfd-51033f5a09b8"), "sccm");


            paramsResponse = await client.GetGuidelineParameters(testClient.Id, glId);
            deployTypeParam = paramsResponse.Parameters.FirstOrDefault(p => p.Id == "bb82ace7-c500-e717-3dfd-51033f5a09b8");

            if(deployTypeParam.Value != "sccm")
            {
                Assert.Fail("Could not set deploy type parameter after guideline creation.");
            }



            await client.DeleteGuideline(testClient.Id, glId);

            GuidelinesResponse responseAfterDelete = await client.GetGuidelines(testClient.Id);

            newGuideline = responseAfterDelete.Guidelines.FirstOrDefault(x => x.GuidelineID == glId);

            if (newGuideline != null) Assert.Fail("new guideline not deleted");
        }


        [TestMethod]
        public async Task Should_ClearGuidelines()
        {
            GuidelinesResponse response = await client.GetGuidelines(testClient.Id);

        }




        [TestMethod]
        public async Task Should_GetCatalogItemParameters()
        {
            // Adobe After Effects
            Guid catalogItemId = Guid.Parse("6fcab492-3075-4335-9825-4a3c70409995");

            PackageParametersResponse response = await client.GetSoftwareCatalogParameters(testClient.Id, catalogItemId);
            Assert.IsNotNull(response.Parameters);

            if (response.Parameters.Length == 0)
            {
                Assert.Inconclusive("Catalog Item has no parameters.");
            }


            foreach (var parameter in response.Parameters)
            {
                Debug.WriteLine($"Name: '{parameter.Name}' Id:'{parameter.Id}' Value: '{parameter.FieldValue}' DefaultValue: '{parameter.DefaultValue}' ");
            }
        }


        [TestMethod]
        public async Task Should_SetCatalogItemParameters()
        {
            // Chrome
            Guid catalogItemId = Guid.Parse("6fcab492-3075-4335-9825-4a3c70409995");

            PackageParametersResponse response = await client.GetSoftwareCatalogParameters(testClient.Id, catalogItemId);
            Assert.IsNotNull(response.Parameters);

            if (response.Parameters.Length == 0)
            {
                Assert.Inconclusive("Catalog Item has no parameters.");
            }

            PackageParameter homePageParameter = response.Parameters.FirstOrDefault(p => p.Name == "HomePage");

            if (homePageParameter == null)
            {
                Assert.Inconclusive("Homepage Parameter not found.");
            }

            string testValue = "www.mytesturl.com";
            string oldHomepage = homePageParameter.FieldValue;

            SetPackageParameterResponse setResponse = await client.SetSoftwareCatalogParameter(testClient.Id, catalogItemId, Guid.Parse(homePageParameter.Id), testValue);

            PackageParametersResponse newValueGetResponse = await client.GetSoftwareCatalogParameters(testClient.Id, catalogItemId);

            homePageParameter = newValueGetResponse.Parameters.FirstOrDefault(p => p.Name == "HomePage");

            Assert.IsTrue(testValue == homePageParameter.FieldValue, "Did not set the value of the parameter.");

            SetPackageParameterResponse setBackResponse = await client.SetSoftwareCatalogParameter(testClient.Id, catalogItemId, Guid.Parse(homePageParameter.Id), oldHomepage);



            //foreach (var parameter in response.Parameters)
            //{
            //    Debug.WriteLine($"Name: '{parameter.Name}' Id:'{parameter.Id}' FieldValue: '{parameter.FieldValue}' DefaultValue: '{parameter.DefaultValue}' ");
            //}
        }

        [TestMethod]
        public async Task Should_GetCatalogItemReleases()
        {
            Guid catalogItemId = Guid.Parse("837b537f-30b9-4502-b2d4-15340d0e065f");

            CatalogItemReleaseResponse response = await client.GetSoftwareCatalogRelease(testClient.Id, catalogItemId);
        }

        [TestMethod]
        public async Task Should_SubmitFeedback()
        {
            string email = "labtagonTest@mail.de";
            string phone = "+4916622";
            int priority = 1;
            string description = "Testing ticket Submit";
            string referenceNumbe = "21234Test";
            string response = await client.SubmitFeedback(email,phone,priority,description, referenceNumbe);
            Assert.IsNotNull(response);
            Debug.WriteLine("Ticket was created your Ticketnumber: " + response);
            

        }


        [TestMethod]
        public async Task Should_StartFileUpload()
        {

            var response = await client.StartFileUpload(425,Guid.Empty);
            Assert.IsNotNull(response);
            Debug.WriteLine("I got my FileUpload id " + response.ToString());
        }
  

    }
}