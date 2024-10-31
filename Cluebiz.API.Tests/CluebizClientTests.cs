using Cluebiz.API.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
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
        public async Task Should_GetFullCatalog()
        {
            Dictionary<SoftwareCatalog, List<SoftwareCatalogRelease>> packagesWithReleases = new Dictionary<SoftwareCatalog, List<SoftwareCatalogRelease>>();

            foreach (Cluebiz.API.Contracts.SoftwareCatalog catalogItem in (await client.GetSoftwareCatalog(testClient.Id)).SoftwareCatalog)
            {
                List<SoftwareCatalogRelease> releases = new List<SoftwareCatalogRelease>();
                packagesWithReleases.Add(catalogItem,releases);

                var releaseResponse = (await client.GetSoftwareCatalogRelease(testClient.Id, catalogItem.CatalogId));

                if (releaseResponse.Catalogs != null)
                {
                    foreach (Cluebiz.API.Contracts.SoftwareCatalogRelease release in releaseResponse.Catalogs)
                    {
                        releases.Add(release);
                    }

                }
            }

            var packagesWithoutAnyReleases = packagesWithReleases.Where(x => x.Value.Count == 0);

            var packagesWithoutIncludedReleases = packagesWithReleases.Where(x => 
            !x.Value.Any(r => r.PackageType != "msix" && r.PackageType != "msx" && r.PackageType != "appv"
            && r.PackageType != "macos"));


            var packagesWithoutAnyReleasesCSV ="Name;Id\n" +
                string.Join("\n",packagesWithoutAnyReleases.Select(r => r.Key.CatalogName + ";" + r.Key.CatalogId));

            var packagesWithoutIncludedReleasesCSV = "Name;Id;Releases\n" +
    string.Join("\n", packagesWithoutIncludedReleases.Select(r => r.Key.CatalogName + ";" + r.Key.CatalogId+";"+string.Join(",",r.Value.Select(p=> p.PackageType))));


            int a = 5;

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
            Guid catalogItemId = Guid.Parse("b0670290-e8ba-4de5-af31-37c9cb26af27");

            PackageParametersResponse response = await client.GetSoftwareCatalogParameters(new Guid("e56e4746-5ca6-452c-a333-2cfe272b5681"), catalogItemId);
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
            Guid catalogItemId = Guid.Parse("206c8338-0c3b-4ddd-9648-2f83d9e8c91e");

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
        public async Task Should_SetParameter()
        {
            Guid clientId = Guid.Parse("234de416-ccc7-441b-b666-fe9051e76a06");
            string base64 = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iSVNPLTg4NTktMSI/Pgo8RW1waXJ1bVBhY2thZ2UgVVVJRD0iezBBNkJGN0EwLThBNUMtNEMzRC05QzNBLTMwQzFFODVFMUIwNX0iIE5hbWU9IjNDWCBQaG9uZSBmb3IgV2luZG93cyAoeDg2KSBNVUkgMTYuMDAzLjAwMC4xMTEiIFR5cGU9IlN3UGFja2FnZSIgVmVyc2lvbj0iMS4xIj4KICA8VmVyc2lvbkluZm9ybWF0aW9uIFZlcnNpb249IjE2LjAwMy4wMDAuMTExIiBSZXZpc2lvbj0iMCIvPgogIDxUYXJnZXRQYXRoPjNDWFxQaG9uZSBmb3IgV2luZG93cyAoeDg2KSBNVUlcMTYuMDAzLjAwMC4xMTE8L1RhcmdldFBhdGg+CiAgPERlc2NyaXB0aW9uPjNDWCBQaG9uZSBmb3IgV2luZG93cyAxNiAxNi4zLjAuMTExPC9EZXNjcmlwdGlvbj4KICA8RGF0YWJhc2VTZXR0aW5ncz4KICAgIDxTb2Z0d2FyZT4KICAgICAgPFNvZnR3YXJlSUQ+ezBBNkJGN0EwLThBNUMtNEMzRC05QzNBLTMwQzFFODVFMUIwNX08L1NvZnR3YXJlSUQ+CiAgICAgIDxUeXBlPkFwcDwvVHlwZT4KICAgICAgPFN1YlR5cGU+PC9TdWJUeXBlPgogICAgICA8TmFtZT4zQ1ggUGhvbmUgZm9yIFdpbmRvd3MgKHg4NikgTVVJIDE2LjAwMy4wMDAuMTExPC9OYW1lPgogICAgICA8VmVyc2lvbj4xNi4wMDMuMDAwLjExMTwvVmVyc2lvbj4KICAgICAgPFJldmlzaW9uPjA8L1JldmlzaW9uPgogICAgICA8VGFyZ2V0UGF0aD4zQ1hcUGhvbmUgZm9yIFdpbmRvd3MgKHg4NikgTVVJXDE2LjAwMy4wMDAuMTExPC9UYXJnZXRQYXRoPgogICAgICA8RGVzY3JpcHRpb24+M0NYIFBob25lIGZvciBXaW5kb3dzIDE2IDE2LjMuMC4xMTE8L0Rlc2NyaXB0aW9uPgogICAgICA8RG9jdUZpbGVOYW1lPiVQYWNrYWdlcyVcJVBhY2thZ2VJRCVcTWFudWFsXE1hbnVhbC5wZGY8L0RvY3VGaWxlTmFtZT4KICAgICAgPEluZm9GaWxlPjwvSW5mb0ZpbGU+CiAgICAgIDxJY29uPiVQYWNrYWdlcyVcJVBhY2thZ2VJRCVcSW5zdGFsbFxTZXR1cC5pY288L0ljb24+CiAgICAgIDxWZXJzaW9uRmlsZT48L1ZlcnNpb25GaWxlPgogICAgICA8Q2hlY2tGaWxlPiVQYWNrYWdlcyVcM0NYXFBob25lIGZvciBXaW5kb3dzICh4ODYpIE1VSVwlVmVyc2lvbiVcSW5zdGFsbFxTZXR1cC5pbmY8L0NoZWNrRmlsZT4KICAgICAgPFNjcmlwdD4lUGFja2FnZXMlXDNDWFxQaG9uZSBmb3IgV2luZG93cyAoeDg2KSBNVUlcJVZlcnNpb24lXEluc3RhbGxcU2V0dXAuaW5mPC9TY3JpcHQ+CiAgICAgIDxEaXJlY3Rvcnk+JVBhY2thZ2VzJVwzQ1hcUGhvbmUgZm9yIFdpbmRvd3MgKHg4NikgTVVJXCVWZXJzaW9uJVw8L0RpcmVjdG9yeT4KICAgICAgPFRlc3RlZD5XaW4xMCAoeDY0KSwgV2luMTEgKHg2NCk8L1Rlc3RlZD4KICAgICAgPEluZm9EZXA+PC9JbmZvRGVwPgogICAgICA8SW5mb0NtZExpbmU\r\n+L1MyPC9JbmZvQ21kTGluZT4KICAgICAgPERlcGVuZGVuY2llcz48L0RlcGVuZGVuY2llcz4KICAgICAgPFJlcXVpcmVtZW50cz48L1JlcXVpcmVtZW50cz4KICAgICAgPENtZExpbmU+IiVTZXR1cCUiICVTZXR1cFBhcm1zJSAiJVNjcmlwdCUiIC9TMjwvQ21kTGluZT4KICAgICAgPENoZWNrUmVnPjwvQ2hlY2tSZWc+CiAgICAgIDxQcm9kdWN0S2V5PiRNYXRyaXg0MlBhY2thZ2VzJFwzQ1hcUGhvbmUgZm9yIFdpbmRvd3MgKHg4NikgTVVJPC9Qcm9kdWN0S2V5PgogICAgICA8TWFjaGluZUtleU5hbWU+JE1hdHJpeDQyUGFja2FnZXMkXDNDWFxQaG9uZSBmb3IgV2luZG93cyAoeDg2KSBNVUlcMTYuMDAzLjAwMC4xMTE8L01hY2hpbmVLZXlOYW1lPgogICAgICA8UHJvZHVjdEZpbGU+PC9Qcm9kdWN0RmlsZT4KICAgICAgPEludmVudG9yeUlEPjwvSW52ZW50b3J5SUQ+CiAgICAgIDxBdXRob3I+TGFidGFnb248L0F1dGhvcj4KICAgICAgPENyZWF0aW9uRGF0ZT4yNC4wNy4yMDI0PC9DcmVhdGlvbkRhdGU+CiAgICAgIDxNZXRob2Q+VW5hdHRlbmRlZDwvTWV0aG9kPgogICAgICA8TGFzdENoYW5nZT4yNC4wNy4yMDI0PC9MYXN0Q2hhbmdlPgogICAgICA8QnVpbGQ+MDwvQnVpbGQ+CiAgICAgIDxQYWNrYWdlRWRpdG9yUGF0aD48L1BhY2thZ2VFZGl0b3JQYXRoPgogICAgICA8QURHcm91cD48L0FER3JvdXA+CiAgICAgIDxDYXRlZ29yeT5BbGxnZW1laW48L0NhdGVnb3J5PgogICAgICA8T1NTeXN0ZW1zPjEwMTA4OTI4MDwvT1NTeXN0ZW1zPgogICAgICA8T1NMaXN0Pldpbjh4LCBXaW4xMCwgV2luMTEsIFdpbjIwMjIsIFdpbjIwMTksIFdpbjIwMTYsIFdpbjIwMTJSMiwgV2luMjAwOFIyLCBXaW43PC9PU0xpc3Q+CiAgICAgIDxPU0NhcGFiaWxpdGllcz54NjQ8L09TQ2FwYWJpbGl0aWVzPgogICAgICA8U0VRVUVOQ0U+MTwvU0VRVUVOQ0U+CiAgICAgIDxFeHRlcm5hbFNldHVwPmZhbHNlPC9FeHRlcm5hbFNldHVwPgogICAgICA8Q2FsbEFzeW5jaHJvbm91cz5mYWxzZTwvQ2FsbEFzeW5jaHJvbm91cz4KICAgICAgPERpc3RyaWJ1dGlvbj5mYWxzZTwvRGlzdHJpYnV0aW9uPgogICAgICA8QWxsb3dVbmluc3RhbGxhdGlvbj50cnVlPC9BbGxvd1VuaW5zdGFsbGF0aW9uPgogICAgICA8RGlzY29udGludWU+ZmFsc2U8L0Rpc2NvbnRpbnVlPgogICAgICA8Ykluc3RhbGxSZWFkeT50cnVlPC9iSW5zdGFsbFJlYWR5PgogICAgICA8RHVyYXRpb24+MTwvRHVyYXRpb24+CiAgICAgIDxMaWNlbmNlcz4tMTwvTGljZW5jZXM+CiAgICAgIDxQcm9jZXNzb3JTcGVlZD4tMTwvUHJvY2Vzc29yU3BlZWQ+CiAgICAgIDxMaWNlbmNlcz4tMTwvTGljZW5jZXM+CiAgICAgIDxEaXNrRnJlZUFwcGxpY2F0aW9uPi0xPC9EaXNrRnJlZUFwcGxpY2F0aW9uPgogICAgICA8RGlza0ZyZWVTeXN0ZW0+LTE8L0Rpc2tGcmVlU3lzdGVtPgogICAgICA8UkFNPi0xPC9SQU0+CiAgICAgIDxNaW5CYW5kd2lkdGg+MDwvTWluQmFuZHdpZHRoPgogICAgICA8SW5zdGFsbENvbnRleHQ+MDwvSW5zdGFsbENvbnRleHQ+CiAgICAgIDxUb2tlbkl\r\nkPjwvVG9rZW5JZD4KICAgICAgPFRva2VuVHRsPjA8L1Rva2VuVHRsPgogICAgICA8UmVnaXN0ZXJOYW1lPkxhYnRhZ29uIFBhY2thZ2UgQ2xvdWQ8L1JlZ2lzdGVyTmFtZT4KICAgICAgPFJlZ2lzdGVyVGV4dD5MYWJ0YWdvbiBQYWNrYWdlIENsb3VkPC9SZWdpc3RlclRleHQ+CiAgICAgIDxSZWdpc3Rlck9TTGlzdD5XaW44eCwgV2luMTAsIFdpbjExPC9SZWdpc3Rlck9TTGlzdD4KICAgICAgPFJlZ2lzdGVyT1NDYXBhYmlsaXRpZXM+eDY0PC9SZWdpc3Rlck9TQ2FwYWJpbGl0aWVzPgogICAgPC9Tb2Z0d2FyZT4KICAgIDxTd0RlcGVuZGVuY2llcy8+CiAgICA8U1dBVVRGaWxlcy8+CiAgICA8U1dTZXF1ZW5jZXMvPgogICAgPFBhY2thZ2VWYXJpYWJsZXM+CiAgICA8VmFyaWFibGUgTnVtYmVyPSIxIj4KICAgICAgICAgICAgPFZhcmlhYmxlTmFtZT5SZW1vdmVFeGlzdGluZ1NvZnR3YXJlPC9WYXJpYWJsZU5hbWU+CiAgICAgICAgICAgIDxWYXJpYWJsZVR5cGU+MDwvVmFyaWFibGVUeXBlPgogICAgICAgICAgICA8RGVzY3JpcHRpb24+UmVtb3ZlRXhpc3RpbmdTb2Z0d2FyZTwvRGVzY3JpcHRpb24+CiAgICAgICAgICAgIDxDb21ib1R5cGU+MDwvQ29tYm9UeXBlPgogICAgICAgICAgICA8TG93ZXJWYWx1ZT48L0xvd2VyVmFsdWU+CiAgICAgICAgICAgIDxVcHBlclZhbHVlPjwvVXBwZXJWYWx1ZT4KICAgICAgICAgICAgPERlZmF1bHRWYWx1ZT5kd2R3ZndmPC9EZWZhdWx0VmFsdWU+CiAgICAgICAgICAgIDxBbGxvd051bGw+MTwvQWxsb3dOdWxsPgogICAgICAgICAgICA8U2hvd0ZsYWc+MDwvU2hvd0ZsYWc+CiAgICAgICAgICAgIDxDb2xsZWN0aW9uPjA8L0NvbGxlY3Rpb24+CiAgICA8L1ZhcmlhYmxlPgogICAgPFZhcmlhYmxlIE51bWJlcj0iMiI+CiAgICAgICAgICAgIDxWYXJpYWJsZU5hbWU+UGF0Y2hFbXBpcnVtRm9sZGVyPC9WYXJpYWJsZU5hbWU+CiAgICAgICAgICAgIDxWYXJpYWJsZVR5cGU+MDwvVmFyaWFibGVUeXBlPgogICAgICAgICAgICA8RGVzY3JpcHRpb24+UGF0Y2hFbXBpcnVtRm9sZGVyPC9EZXNjcmlwdGlvbj4KICAgICAgICAgICAgPENvbWJvVHlwZT4wPC9Db21ib1R5cGU+CiAgICAgICAgICAgIDxMb3dlclZhbHVlPjwvTG93ZXJWYWx1ZT4KICAgICAgICAgICAgPFVwcGVyVmFsdWU+PC9VcHBlclZhbHVlPgogICAgICAgICAgICA8RGVmYXVsdFZhbHVlPjwvRGVmYXVsdFZhbHVlPgogICAgICAgICAgICA8QWxsb3dOdWxsPjE8L0FsbG93TnVsbD4KICAgICAgICAgICAgPFNob3dGbGFnPjA8L1Nob3dGbGFnPgogICAgICAgICAgICA8Q29sbGVjdGlvbj4wPC9Db2xsZWN0aW9uPgogICAgPC9WYXJpYWJsZT4KICAgIDwvUGFja2FnZVZhcmlhYmxlcz4KICA8L0RhdGFiYXNlU2V0dGluZ3M+CjwvRW1waXJ1bVBhY2thZ2U+Cg==";
            Guid softwareCatalogId = Guid.Parse("b0670290-e8ba-4de5-af31-37c9cb26af27");
            int length = 4324;
            Guid parameterId = Guid.Parse("75a9da11-90a4-48e3-87cb-24b68f7b6fb9");
            Guid fileId = await client.StartFileUpload(length, clientId);

            await client.FileChunkUpload(fileId, base64, clientId);

            await client.SetSoftwareParameterFile(clientId, softwareCatalogId, parameterId, fileId);


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