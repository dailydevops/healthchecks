namespace NetEvolve.HealthChecks.Azure.Files;

using System;
using global::Azure.Identity;
using global::Azure.Storage.Files.Shares;

/// <summary>
/// Describes the mode used to create the <see cref="ShareServiceClient"/>.
/// </summary>
public enum FileClientCreationMode
{
    /// <summary>
    /// The default mode. The <see cref="ShareServiceClient"/> is loading the preregistered instance from the <see cref="IServiceProvider"/>.
    /// </summary>
    ServiceProvider = 0,

    /// <summary>
    /// The <see cref="ShareServiceClient"/> is created using the <see cref="DefaultAzureCredential"/>.
    /// </summary>
    DefaultAzureCredentials = 1,

    /// <summary>
    /// The <see cref="ShareServiceClient"/> is created using the connection string.
    /// </summary>
    ConnectionString = 2,

    /// <summary>
    /// The <see cref="ShareServiceClient"/> is created using the account name and account key.
    /// </summary>
    SharedKey = 3,

    /// <summary>
    /// The <see cref="ShareServiceClient"/> is created using the service URI with SAS token.
    /// </summary>
    AzureSasCredential = 4,
}
