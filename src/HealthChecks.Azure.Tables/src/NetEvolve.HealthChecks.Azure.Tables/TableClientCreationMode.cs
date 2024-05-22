namespace NetEvolve.HealthChecks.Azure.Tables;

using System;
using global::Azure.Data.Tables;
using global::Azure.Identity;

/// <summary>
/// Describes the mode used to create the <see cref="TableServiceClient"/>.
/// </summary>
public enum TableClientCreationMode
{
    /// <summary>
    /// The default mode. The <see cref="TableServiceClient"/> is loading the preregistered instance from the <see cref="IServiceProvider"/>.
    /// </summary>
    ServiceProvider = 0,

    /// <summary>
    /// The <see cref="TableServiceClient"/> is created using the <see cref="DefaultAzureCredential"/>.
    /// </summary>
    DefaultAzureCredentials = 1,

    /// <summary>
    /// The <see cref="TableServiceClient"/> is created using the <see cref="TableClientAvailableOptions.ConnectionString"/>.
    /// </summary>
    ConnectionString = 2,

    /// <summary>
    /// The <see cref="TableServiceClient"/> is created using the <see cref="TableClientAvailableOptions.AccountName"/>
    /// and <see cref="TableClientAvailableOptions.AccountKey"/>. As well as the <see cref="TableClientAvailableOptions.ServiceUri"/>.
    /// </summary>
    SharedKey = 3,

    /// <summary>
    /// The <see cref="TableServiceClient"/> is created using the <see cref="TableClientAvailableOptions.ServiceUri"/>.
    /// </summary>
    AzureSasCredential = 4
}
