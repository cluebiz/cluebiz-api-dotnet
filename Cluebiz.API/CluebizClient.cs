using Cluebiz.API.Constants;
using Cluebiz.API.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace Cluebiz.API
{

    /// <summary>
    /// Implements convenient access to the cluebiz API.
    /// </summary>
    internal class CluebizClient : CluebizClientBase, ICluebizClient
    {

        /// <summary>
        /// Creates a new cluebiz client, given correct secrets.
        /// </summary>
        /// <param name="serverAddress">Endpoint of the cluebiz API.</param>
        /// <param name="userId">The cluebiz API user id.</param>
        /// <param name="key">Secret key, belonging to the userId.</param>
        internal CluebizClient(string serverAddress, string userId, string key)
            : base(serverAddress, userId, key)
        {
        }

        public Task<ClientResponse> GetClients()
            => Get<ClientResponse>("getClient");


        public Task<CVEResponse> GetCVEs(Guid clientId)
            => Get<CVEResponse>("GETSOFTWARECATALOGCVES", clientId);

        public Task<GuidelinesResponse> GetGuidelines(Guid clientId)
            => Get<GuidelinesResponse>("getGuideline", clientId);


        public Task<CatalogResponse> GetSoftwareCatalog(Guid clientId)
            => Get<CatalogResponse>("getSoftwareCatalog", clientId, HttpUtility.ParseQueryString("detaillevel=high"));



        public Task<GetSoftwareResponse> GetSoftware(Guid clientId)
            => Get<GetSoftwareResponse>("getSoftware", clientId);


        public Task<LicenseResponse> GetLicenses(Guid clientId)
            => Get<LicenseResponse>("GETLICENSE", clientId);


        public async Task SetLicense(Guid clientId, Guid licenseId, string licenseType, DateTime? validUntil)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            query["licenseId"] = licenseId.ToString();
            query["licenseType"] = licenseType;
            query["endDate"] = validUntil.ToString();
            await Get<LicenseResponse>("SETLICENSE", clientId, query);
        }


        public Task<CatalogItemReleaseResponse> GetSoftwareCatalogRelease(Guid clientId, Guid softwareCatalogId)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            query["softwareCatalogId"] = softwareCatalogId.ToString();
            return Get<CatalogItemReleaseResponse>("getSoftwareCatalogReleases", clientId, query);
        }


        public Task<PackageParametersResponse> GetSoftwareCatalogParameters(Guid clientId, Guid softwareCatalogId, Guid? guidelineId = null)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            query["softwareCatalogId"] = softwareCatalogId.ToString();

            if(guidelineId.HasValue)
                query["guidelineId"] = guidelineId.ToString();

            return Get<PackageParametersResponse>("GETSOFTWARECATALOGPARAMETERS", clientId, query);
        }

        public Task<SetPackageParameterResponse> SetSoftwareCatalogParameter(Guid clientId, Guid softwareCatalogId, Guid softwareCatalogParameterId, string fieldValue, Guid? guidelineId = null)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            query["softwareCatalogId"] = softwareCatalogId.ToString();

            query["softwareCatalogId"] = softwareCatalogId.ToString();
            query["softwareCatalogParameterId"] = softwareCatalogParameterId.ToString();
            query["fieldValue"] = fieldValue.ToString();

            if (guidelineId.HasValue)
                query["guidelineId"] = guidelineId.ToString();

            return Get<SetPackageParameterResponse>("SETSOFTWARECATALOGPARAMETER", clientId, query);
        }


        public Task<CatalogReleaseDownloadResponse> GetSoftwareCatalogDownloadLink(Guid clientId, Guid softwareCatalogDeployId, Guid guidelineId)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            query["softwareCatalogDeployId"] = softwareCatalogDeployId.ToString();
            query["guidelineId"] = guidelineId.ToString();
            return Get<CatalogReleaseDownloadResponse>("getSoftwareCatalogDownloadLink", clientId, query);
        }


        public async Task<Guid> StartBakingProcess(Guid clientId, Guid? softwareId, Guid guidelineId, bool isInidividual)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            if (isInidividual)
                query["softwareDeployId"] = softwareId.ToString();
            else
                query["softwareCatalogDeployId"] = softwareId.ToString();

            query["guidelineId"] = guidelineId.ToString();
            return (await Get<StartBakingProcessResponse>("SETBAKINGPROCESS", clientId, query)).ProcessId;
        }


        public Task<GetBakingProcessesResponse> GetBakingProcesses(Guid clientId)
        {
            return Get<GetBakingProcessesResponse>("GETBAKINGPROCESS", clientId);
        }


        public Task<CatalogReleaseDownloadResponse> GetSoftwareDownloadLink(Guid clientId, Guid softwareId, Guid guidelineId)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            query["softwareId"] = softwareId.ToString();
            query["guidelineId"] = guidelineId.ToString();
            return Get<CatalogReleaseDownloadResponse>("GETSOFTWAREDOWNLOADLINK", clientId, query);
        }


        public async Task<string?> GetPackageRobotSessionToken(Guid clientId, Guid guidelineId)
        {
            RobotSessionTokensResponse response = await Get<RobotSessionTokensResponse>("getSimpleToken", clientId);
            return response.Tokens?
                .OrderByDescending(t => t.EndDate)
                .FirstOrDefault(t => t.GuidelineId == guidelineId && t.TokenType == "robot")
                ?.Token;
        }


        public async Task<string> RequestPackageRobotSessionToken(Guid clientId, Guid guidelineId)
        {
            NameValueCollection query = HttpUtility.ParseQueryString("tokenType=robot");
            query["guidelineId"] = guidelineId.ToString();
            return (await Get<RobotSessionTokenResponse>("requestSimpleToken", clientId, query)).Token;
        }


        public async Task<Guid?> CreateSoftwareOrder(Guid clientId, string manufacturer, string productName, string version)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            query["product"] = productName;
            query["manufacturer"] = manufacturer;
            query["releasename"] = version;
            return (await Get<CreateOrderResponse>("addOrder", clientId, query)).Id;
        }


        public Task<GuidelineParametersResponse> GetGuidelineParameters(Guid clientId, Guid guidelineId)
        {
            NameValueCollection query = HttpUtility.ParseQueryString("language=german");
            query["guidelineId"] = guidelineId.ToString();
            return Get<GuidelineParametersResponse>("getGuidelineParameter", clientId, query);
        }


        public Task<GetOrderDetailsResponse> GetOrderDetails(Guid clientId, Guid orderId)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            query["orderId"] = orderId.ToString();
            return Get<GetOrderDetailsResponse>("GETORDERDETAIL", clientId, query);
        }


        public Task<GetOrderResponse> GetOrders(Guid clientId)
            => Get<GetOrderResponse>("getorder", clientId);



        public async Task RemoveOrder(Guid clientId, Guid orderId)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            query["orderId"] = orderId.ToString();
            await Get("removeOrder", null, query);
        }


        public async Task<Guid?> CreateClient(string clientName)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            query["clientName"] = clientName;
            query["publicUrl"] = serverUrl;
            return (await Get<CreateClientResponse>("addClient", null, query)).Id;
        }


        public Task RemoveClient(Guid clientId)
            => Get("removeClient", clientId);


        /// <summary>
        /// For setting deployment type
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="guidelineId"></param>
        /// <param name="parameterId"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        public Task SetGuidelineParameter(Guid clientId, Guid guidelineId, Guid parameterId, string parameterValue)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            query["guidelineId"] = guidelineId.ToString();
            query["guidelineParameterId"] = parameterId.ToString();
            query["guidelineParameterValue"] = parameterValue;
            return Get("SETGUIDELINEPARAMETER", clientId, query);
        }

        /// <summary>
        /// For setting non-deployment type
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="guidelineId"></param>
        /// <param name="parameterId"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        public Task SetGuidelineParameter(Guid clientId, Guid guidelineId, string parameterId, string parameterValue)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            query["guidelineId"] = guidelineId.ToString();
            query["guidelineParameterId"] = parameterId.ToString();
            query["guidelineParameterValue"] = parameterValue;
            return Get("SETGUIDELINEPARAMETER", clientId, query);

        }


        public Task<GetCatalogItemInvoicesResponse> GetCatalogItemInvoices(Guid clientId, Guid? invoiceId = null)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            if (invoiceId.HasValue)
                query["invoiceId"] = invoiceId.ToString();
            return Get<GetCatalogItemInvoicesResponse>("GETINVOICE", clientId, query);
        }


        public Task<RemoveCatalogItemInvoiceResponse> RemoveCatalogItemInvoice(Guid clientId, Guid invoiceId)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            query["invoiceId"] = invoiceId.ToString();
            return Get<RemoveCatalogItemInvoiceResponse>("REMOVEINVOICE", clientId, query);
        }


        public Task<CreateCatalogItemInvoiceResponse> CreateCatalogItemInvoice(
            Guid clientId,
            Guid softwareCatalogDeployId,
            string? invoiceproducttitle = null,
            Guid? invoiceId = null
            )
        {
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            query["invoiceProductId"] = InvoiceProduct.SinglePackageOneYear.ToString();
            query["softwareCatalogDeployId"] = softwareCatalogDeployId.ToString();
            query["invoiceproducttitle"] = invoiceproducttitle;
            if (invoiceId.HasValue)
                query["invoiceId"] = invoiceId.ToString();

            return Get<CreateCatalogItemInvoiceResponse>("ADDINVOICEITEM", clientId, query);
        }


        public async Task<Guid> CreateGuideline(Guid clientId, string title)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            query["guidelineTitle"] = title;
            return (await Get<CreateGuidelineResponse>("ADDGUIDELINE", clientId, query)).GuidelineId;
        }

        public async Task DeleteGuideline (Guid clientId, Guid guidelineId)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            query["guidelineId"] = guidelineId.ToString();
            await Get("REMOVEGUIDELINE",clientId,query);
        }

        public async Task<string> SubmitFeedback(string email, string phone, int priority, string description ,string refernceNumber)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            query["ownerlogin"] = email;
            query["ownerphone"] = phone;
            query["referencenumber"] = refernceNumber;
            query["Priority"] = priority.ToString();
            query["Description"] = description;
            return (await Get<SubmitFeedbackResponse>("ADDFEEDBACK", null, query)).TicketNumber;

        }

        public async Task<Guid> StartFileUpload(int fileSize,Guid clientId)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            query["fileSize"] = fileSize.ToString();
            query["clientId"] = clientId.ToString();
            var x = (await Get<StartFileUploadResponse>("FILEUPLOADSTART", null, query));
            return x.FileId;

        }

        public async Task FileChunkUpload(Guid fileId, string data, Guid clientId)
        {

            await PostChunks(data, fileId, clientId);

        }

        public async Task SetSoftwareParameterFile( Guid softwareCatalogId, Guid softwareCatalogParameterId, Guid fileId, Guid clientId, Guid? guidelineId =null)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);

            query["softwareCatalogId"] = softwareCatalogId.ToString();
            query["softwareCatalogParameterId"] = softwareCatalogParameterId.ToString();
            query["fileId"] = fileId.ToString();
            if(guidelineId!= null) query["guidelineId"] = guidelineId.ToString();
            await Get("SETSOFTWARECATALOGPARAMETERFILE", clientId, query);
        }

       
    }
    
}
