using Cluebiz.API.Contracts;
using System;
using System.Threading.Tasks;

namespace Cluebiz.API
{

    /// <summary>
    /// Communication with the Cluebiz API.
    /// </summary>
    public interface ICluebizClient
    {
        #region Customer Client

        /// <summary>
        /// Returns all customer clients.
        /// </summary>
        /// <returns>Clients</returns>
        Task<ClientResponse> GetClients();

        /// <summary>
        /// Creates a new customer client.
        /// </summary>
        /// <param name="clientName">Name of the customer.</param>
        /// <returns>Customer Id, or null if clientName already exists.</returns>
        Task<Guid?> CreateClient(string clientName);

        /// <summary>
        /// Deletes a customer client completely, without an option for recovery!
        /// </summary>
        /// <param name="clientId">Customer client to delete.</param>
        /// <returns></returns>
        Task RemoveClient(Guid clientId);

        #endregion

        #region Catalog

        /// <summary>
        /// Gets all catalog Items in the package shop, for a customer client.
        /// </summary>
        /// <param name="clientId">Customer client</param>
        /// <returns>Software Catalog</returns>
        Task<CatalogResponse> GetSoftwareCatalog(Guid clientId);

        /// <summary>
        /// Gets releases of a software catalog item, for a customer client.
        /// </summary>
        /// <param name="clientId">Customer client.</param>
        /// <param name="softwareCatalogId">Software Catalog Item</param>
        /// <returns>Releases</returns>
        Task<CatalogItemReleaseResponse> GetSoftwareCatalogRelease(Guid clientId, Guid softwareCatalogId);

        /// <summary>
        /// Returns all CVEs.
        /// </summary>
        /// <param name="clientId">Customer client, for whom to get the CVE list.</param>
        /// <returns>CVEs</returns>
        Task<CVEResponse> GetCVEs(Guid clientId);

        /// <summary>
        /// Gets parameters of a software catalog item, for a customer client.
        /// </summary>
        /// <param name="clientId">Customer client</param>
        /// <param name="softwareCatalogId">The software catalog item</param>
        /// <param name="guidelineId">Optionally, for a specific guideline, to include parameters of origin: guidelien</param>
        /// <returns>Parameters</returns>
        Task<PackageParametersResponse> GetSoftwareCatalogParameters(Guid clientId, Guid softwareCatalogId, Guid? guidelineId = null);

        /// <summary>
        /// Sets a software catalog item paremeter, for a customer client.
        /// </summary>
        /// <param name="clientId">Customer Client</param>
        /// <param name="softwareCatalogId">Catalog Item</param>
        /// <param name="softwareCatalogParameterId">Parameter to set</param>
        /// <param name="fieldValue">New Value to set.</param>
        /// <param name="guidelineId">Optionally, for a specific guideline, to set parameters of origin: guidelien</param>
        /// <returns></returns>
        Task<SetPackageParameterResponse> SetSoftwareCatalogParameter(Guid clientId, Guid softwareCatalogId, Guid softwareCatalogParameterId, string fieldValue, Guid? guidelineId = null);



        #endregion

        #region IndividualSoftware

        /// <summary>
        /// Gets all individual software for a customer client.
        /// This means all custom packaging for this customer.
        /// </summary>
        /// <param name="clientId">The customer client.</param>
        /// <returns>Individual Software</returns>
        Task<GetSoftwareResponse> GetSoftware(Guid clientId);

        #endregion

        #region Individual Software Orders

        /// <summary>
        /// Gets all orders of a customer client. Use <see cref="GetOrderDetails"/> for additional information, that is not included here.
        /// </summary>
        /// <param name="clientId">Customer client</param>
        /// <returns>Orders</returns>
        Task<GetOrderResponse> GetOrders(Guid clientId);

        /// <summary>
        /// Gets details of an order, belonging to a customer client.
        /// </summary>
        /// <param name="clientId">Customer client.</param>
        /// <param name="orderId">Order</param>
        /// <returns>Order details.</returns>
        Task<GetOrderDetailsResponse> GetOrderDetails(Guid clientId, Guid orderId);

        /// <summary>
        /// Creates an initial order for individual packaging. (Individual software).
        /// This order is a sales operation and will be billed.
        /// Additional information about the software must be recorded with help of the order wizard.
        /// To get the status of the order, <see cref="GetOrderDetails"/>
        /// </summary>
        /// <param name="clientId">Customer client.</param>
        /// <param name="manufacturer">Individual software manufacturer.</param>
        /// <param name="productName">Individual software product name.</param>
        /// <param name="version">Individual software version to package.</param>
        /// <returns>Order Id. returns null if the order could not be created (because an identical order exists.)</returns>
        Task<Guid?> CreateSoftwareOrder(Guid clientId, string manufacturer, string productName, string version);

        /// <summary>
        /// Delets an order. Since an order is billed, use this only for orders in the "temp" status <see cref="Order.Status"/>
        /// </summary>
        /// <param name="clientId">Customer client</param>
        /// <param name="orderId">Order</param>
        /// <returns></returns>
        Task RemoveOrder(Guid clientId, Guid orderId);
        #endregion

        #region Invoices

        /// <summary>
        /// Gets all invoices for catalog items. Invoices are single orders of catalog items, for a customer client.
        /// </summary>
        /// <param name="clientId">Customer client.</param>
        /// <param name="invoiceId">Invoice</param>
        /// <returns>Invoices</returns>
        Task<GetCatalogItemInvoicesResponse> GetCatalogItemInvoices(Guid clientId, Guid? invoiceId = null);

        /// <summary>
        /// Creates an invoice for a customer client, for a specific package catalog item.
        /// If Incoice Id is set, this will instead be added as an item to an existing invoice.
        /// This invoice is a sales operation and will be billed.
        /// </summary>
        /// <param name="clientId">Customer client</param>
        /// <param name="softwareCatalogDeployId">Software catalog deploy id.</param>
        /// <param name="invoiceproducttitle">TItle of the ordered product.</param>
        /// <param name="invoiceId">Optional Id of existing invoice.</param>
        /// <returns>Invoice.</returns>
        Task<CreateCatalogItemInvoiceResponse> CreateCatalogItemInvoice(Guid clientId, Guid softwareCatalogDeployId, string invoiceproducttitle = null, Guid? invoiceId = null);

        /// <summary>
        /// Removes an invoice for a catalog item.
        /// </summary>
        /// <param name="clientId">Customer client.</param>
        /// <param name="invoiceId">Invoice</param>
        /// <returns></returns>
        Task<RemoveCatalogItemInvoiceResponse> RemoveCatalogItemInvoice(Guid clientId, Guid invoiceId);

        #endregion

        #region Guidelines

        /// <summary>
        /// Gets all guidelines for a customer client.
        /// </summary>
        /// <param name="clientId">Customer client.</param>
        /// <returns>Guidelines</returns>
        Task<GuidelinesResponse> GetGuidelines(Guid clientId);

        /// <summary>
        /// Delets a specific guideline
        /// </summary>
        /// <param name="clientId">For this client</param>
        /// <param name="guidelineId">Id of the guideline to delete</param>
        /// <returns>Nothing</returns>
        Task DeleteGuideline(Guid clientId, Guid guidelineId);

        /// <summary>
        /// Creates a new guideline for a customer client.
        /// </summary>
        /// <param name="clientId">Customer client</param>
        /// <param name="title">Title of the Guideline</param>
        /// <returns>Guideline Id.</returns>
        Task<Guid> CreateGuideline(Guid clientId, string title);

        /// <summary>
        /// Gets all parameters for a guidline of a customer client.
        /// </summary>
        /// <param name="clientId">Customer client.</param>
        /// <param name="guidelineId">Guideline</param>
        /// <returns>Parameters.</returns>
        Task<GuidelineParametersResponse> GetGuidelineParameters(Guid clientId, Guid guidelineId);


        /// <summary>
        /// Sets a guideline parameter value.
        /// </summary>
        /// <param name="clientId">Customer client.</param>
        /// <param name="guidelineId">Guideline</param>
        /// <param name="parameterId">Parameter</param>
        /// <param name="parameterValue">New Value, to set.</param>
        /// <returns></returns>
        Task SetGuidelineParameter(Guid clientId, Guid guidelineId, Guid parameterId, string parameterValue);



        Task SetGuidelineParameter(Guid clientId, Guid guidelineId, string parameterId, string parameterValue);
        #endregion

        #region DirectDownload

        /// <summary>
        /// Creates a download link for either individual software or software catalog item, for a customer client.
        /// </summary>
        /// <param name="clientId">Customer client.</param>
        /// <param name="softwareCatalogDeployId">release id of a release of the software catalog item.</param>
        /// <param name="guidelineId">The guideline to use for baking the package.</param>
        /// <returns>Downloadlink</returns>
        [Obsolete(message: "Direct download links are unreliable, and the download can time out. Use CluebizClient.StartBakingProcess and CluebizClient.GetBakingProcesses instead.", error: false)]
        Task<CatalogReleaseDownloadResponse> GetSoftwareCatalogDownloadLink(Guid clientId, Guid softwareCatalogDeployId, Guid guidelineId);

        /// <summary>
        /// Get download link for individual software for a customer client.
        /// </summary>
        /// <param name="clientId">Customer client.</param>
        /// <param name="softwareId">Software Catalog Release Id</param>
        /// <param name="guidelineId">Guideline to use for packaging.</param>
        /// <returns>Download link.</returns>
        [Obsolete(message: "Direct download links are unreliable, and the download can time out. Use CluebizClient.StartBakingProcess and CluebizClient.GetBakingProcesses instead.", error: false)]
        Task<CatalogReleaseDownloadResponse> GetSoftwareDownloadLink(Guid clientId, Guid softwareId, Guid guidelineId);
        #endregion

        #region PackagingRequest

        /// <summary>
        /// Enqueues a baking process.
        /// The status of the Baking Process can be got by using <see cref="GetBakingProcesses"></see>
        /// Or by using webhook functionality, to immediately get updated on finish: https://cluebiz.com/help?contentid=4aca1aa8-453f-4c6b-9446-71ab4fd92dc9
        /// </summary>
        /// <param name="clientId">Customer client.</param>
        /// <param name="softwareId">Either if of the individual software, or the release id of a release of the software catalog item.</param>
        /// <param name="guidelineId">The guideline to use for baking the package.</param>
        /// <param name="isInidividual">Set true for individual software, false for catalog item (release)</param>
        /// <returns>Baking Process Id</returns>
        Task<Guid> StartBakingProcess(Guid clientId, Guid? softwareId, Guid guidelineId, bool isInidividual);

        /// <summary>
        /// Gets all current baking processes for a customer client.
        /// </summary>
        /// <param name="clientId">Customer client</param>
        /// <returns>Baking Process Status</returns>
        Task<GetBakingProcessesResponse> GetBakingProcesses(Guid clientId);

        #endregion

        #region Robot

        /// <summary>
        /// Gets the most current robot session token for a customer client, if one exists.
        /// Returns null if no token currently exists.
        /// </summary>
        /// <param name="clientId">Customer client.</param>
        /// <param name="guidelineId">Guideline to use for robot.</param>
        /// <returns>Token, or null.</returns>
        Task<string?> GetPackageRobotSessionToken(Guid clientId, Guid guidelineId);

        /// <summary>
        /// Creates a robot token for a customer client.
        /// If customer has no access to the robot, returns null.
        /// </summary>
        /// <param name="clientId">Customer client</param>
        /// <param name="guidelineId">Guideline to use for robot.</param>
        /// <returns>Token, or null</returns>
        Task<string> RequestPackageRobotSessionToken(Guid clientId, Guid guidelineId);

        #endregion

        #region Licensing

        /// <summary>
        /// Gets all licenses for a customer client.
        /// </summary>
        /// <param name="clientId">Customer client</param>
        /// <returns>Licenses</returns>
        Task<LicenseResponse> GetLicenses(Guid clientId);

        /// <summary>
        /// Sets a license for a customer client.
        /// </summary>
        /// <param name="clientId">The customer client.</param>
        /// <param name="licenseId">Id of the license to set. See list of possible Ids: https://cluebiz.com/help?contentid=10e6a6ff-65fb-4b19-b459-f4addd1aca2c</param>
        /// <param name="licenseType">Type of license to set. See list of possible types: https://cluebiz.com/help?contentid=10e6a6ff-65fb-4b19-b459-f4addd1aca2c</param>
        /// <param name="validUntil">End date of the license validity.</param>
        /// <returns></returns>
        Task SetLicense(Guid clientId, Guid licenseId, string licenseType, DateTime? validUntil);

        #endregion


        #region CustomerInteraction
        /// <summary>
        /// Submit a problem 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="phone"></param>
        /// <param name="priority">1 low, 2 mid,3 high</param>
        /// <param name="description"></param>
        /// <param name="refernceNumber"></param>
        /// <returns></returns>
        Task<string> SubmitFeedback(string email, string phone, int priority,string description, string refernceNumber);


        #endregion
        #region FileHandling
        /// <summary>
        /// Prepare server for incoming Chunks
        /// </summary>
        /// <param name="fileSize"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        Task<Guid> StartFileUpload(int fileSize,Guid clientId);
        /// <summary>
        /// Uploading fileChunks to the server
        /// </summary>
        /// <param name="fileId"> fileId you previously created with StartFileUpload</param>
        /// <param name="data"> data from file as base64</param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        Task FileChunkUpload(Guid fileId, string data,Guid clientId);
        Task SetSoftwareParameterFile(string clientId, Guid softwareCatalogId, Guid softwareCatalogParameterId, Guid fileId, Guid? guidelineId = null);

        #endregion
    }
}