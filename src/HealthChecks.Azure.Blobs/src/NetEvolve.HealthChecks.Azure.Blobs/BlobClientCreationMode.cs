namespace NetEvolve.HealthChecks.Azure.Blobs;

using System;
using global::Azure.Identity;
using global::Azure.Storage.Blobs;

/// <summary>
/// Describes the mode used to create the <see cref="BlobServiceClient"/>.
/// </summary>
public enum BlobClientCreationMode
{
    /// <summary>
    /// The default mode. The <see cref="BlobServiceClient"/> is loading the preregistered instance from the <see cref="IServiceProvider"/>.
    /// </summary>
    ServiceProvider = 0,

    /// <summary>
    /// The <see cref="BlobServiceClient"/> is created using the <see cref="DefaultAzureCredential"/>.
    /// </summary>
    DefaultAzureCredentials = 1,

    /// <summary>
    /// The <see cref="BlobServiceClient"/> is created using the <see cref="BlobContainerAvailableOptions.ConnectionString"/>.
    /// </summary>
    ConnectionString = 2,

    /// <summary>
    /// The <see cref="BlobServiceClient"/> is created using the <see cref="BlobContainerAvailableOptions.AccountName"/>
    /// and <see cref="BlobContainerAvailableOptions.AccountKey"/>. As well as the <see cref="BlobContainerAvailableOptions.ServiceUri"/>.
    /// </summary>
    SharedKey = 3,

    /// <summary>
    /// The <see cref="BlobServiceClient"/> is created using the <see cref="BlobContainerAvailableOptions.ServiceUri"/>.
    /// </summary>
    AzureSasCredential = 4
}
