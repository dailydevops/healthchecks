namespace NetEvolve.HealthChecks.Azure.Kusto;

using System;
using Kusto.Data.Common;

/// <summary>
/// Describes the mode used to create the Kusto client.
/// </summary>
public enum KustoClientCreationMode
{
    /// <summary>
    /// The default mode. The Kusto client is loading the preregistered instance from the <see cref="IServiceProvider"/>.
    /// </summary>
    ServiceProvider = 0,

    /// <summary>
    /// The Kusto client is created using the <see cref="KustoOptions.ConnectionString"/>.
    /// </summary>
    ConnectionString = 1,

    /// <summary>
    /// The Kusto client is created using Azure AD authentication with default credentials.
    /// </summary>
    DefaultAzureCredentials = 2,
}
