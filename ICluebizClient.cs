using Labtagon.Cloud.Packages.CluebizClient.Contracts;
using System;
using System.Threading.Tasks;

namespace Labtagon.Cloud.Packages.CluebizClient
{
    public interface ICluebizClient
    {
        /// <summary>
        /// Gets all clients for this user.
        /// </summary>
        /// <returns>List of clients. Can be null for faulty request.</returns>
        Task<ClientResponse> GetClients();

        /// <summary>
        /// Deletes the complete invoice, with all invoice items.
        /// </summary>
        /// <param name="clientId">Client to which the invoice belongs</param>
        /// <param name="invoiceId">Id of the invoice to delete.</param>
        /// <returns></returns>
        Task<RemoveCatalogItemInvoiceResponse> RemoveCatalogItemInvoice(Guid clientId, Guid invoiceId);

        /// <summary>
        /// Gets a list of all invoices for the client.
        /// </summary>
        /// <param name="clientId">Client id</param>
        /// <param name="invoiceId">Optional, get all when null, only the invoice when set.</param>
        /// <returns></returns>
        Task<GetCatalogItemInvoicesResponse> GetCatalogItemInvoices(Guid clientId, Guid? invoiceId = null);

        /// <summary>
        /// Creates a new invoice, or a new invoice item.
        /// </summary>
        /// <param name="clientId">Client for which to create this.</param>
        /// <param name="softwareCatalogDeployId">The package Item id.</param>
        /// <param name="invoiceproducttitle">Title of the package item.</param>
        /// <param name="invoiceId">Optional, when given will add a new item to the existing invoice.</param>
        /// <returns></returns>
        Task<CreateCatalogItemInvoiceResponse> CreateCatalogItemInvoice(Guid clientId, Guid softwareCatalogDeployId, string invoiceproducttitle = null, Guid? invoiceId = null);

        Task<GuidelinesResponse> GetGuidelines(Guid clientId);
        Task<CatalogResponse> GetSoftwareCatalog(Guid clientId);
        Task<CatalogReleaseDownloadResponse> GetSoftwareCatalogDownloadLink(Guid clientId, Guid softwareCatalogDeployId, Guid guidelineId);
        Task<CatalogItemReleaseResponse> GetSoftwareCatalogRelease(Guid clientId, Guid softwareCatalogId);


        /// <summary>
        /// Creates a new order. OrderId Can be null, when identical order already existed.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="manufacturer"></param>
        /// <param name="productName"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        Task<Guid?> CreateSoftwareOrder(Guid clientId, string manufacturer, string productName, string version);

        Task<GetOrderResponse> GetOrders(Guid clientId);

        Task<GetOrderDetailsResponse> GetOrderDetails(Guid clientId, Guid orderId);

        Task RemoveOrder(Guid clientId, Guid orderId);

        /// <summary>
        /// Gets a session token for the package robot.
        /// This does not generate new tokens, only gets them.
        /// </summary>
        /// <param name="clientId">Client Id</param>
        /// <returns>The session Token.</returns>
        Task<string?> GetPackageRobotSessionToken(Guid clientId, Guid guidelineId);

        /// <summary>
        /// Creates a session token for the package robot.
        /// This does not generate new tokens, only gets them.
        /// </summary>
        /// <param name="clientId">Client Id</param>
        /// <returns>The session Token.</returns>
        Task<string> RequestPackageRobotSessionToken(Guid clientId, Guid guidelineId);

        /// <summary>
        /// Creates a new client.
        /// </summary>
        /// <param name="clientName"></param>
        /// <returns>Id of new client. Null, if could not be created / already exists (name must be unique).</returns>
        Task<Guid?> CreateClient(string clientName);

        Task<CVEResponse> GetCVEs(Guid clientId);

        Task<PackageParametersResponse> GetSoftwareCatalogParameters(Guid clientId, Guid softwareCatalogId);

        Task<LicenseResponse> GetLicenses(Guid clientId);
        Task SetLicense(Guid clientId, Guid licenseId, string licenseType, DateTime? validUntil);

        Task<GetSoftwareResponse> GetSoftware(Guid clientId);

        Task<CatalogReleaseDownloadResponse> GetSoftwareDownloadLink(Guid clientId, Guid softwareId, Guid guidelineId);
        Task<Guid> StartBakingProcess(Guid clientId, Guid? softwareId, Guid guidelineId, bool isInidividual);
        Task<GetBakingProcessesResponse> GetBakingProcesses(Guid clientId);
        Task<GuidelineParametersResponse> GetGuidelineParameters(Guid clientId, Guid guidelineId);
        Task<Guid> CreateGuideline(Guid clientId, string title);
        Task SetGuidelineParameter(Guid clientId, Guid guidelineId, string parameterId, string parameterValue);
        Task RemoveClient(Guid clientId);
    }
}