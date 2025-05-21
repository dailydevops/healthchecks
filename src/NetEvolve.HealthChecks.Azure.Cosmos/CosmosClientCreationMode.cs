namespace NetEvolve.HealthChecks.Azure.Cosmos;

using System;
using global::Azure.Identity;
using Microsoft.Azure.Cosmos;

/// <summary>
/// Describes the mode used to create the <see cref="CosmosClient"/>.
/// </summary>
public enum CosmosClientCreationMode
{
    /// <summary>
    /// The default mode. The <see cref="CosmosClient"/> is loading the preregistered instance from the <see cref="IServiceProvider"/>.
    /// </summary>
    ServiceProvider = 0,

    /// <summary>
    /// The <see cref="CosmosClient"/> is created using the <see cref="DefaultAzureCredential"/>.
    /// </summary>
    DefaultAzureCredentials = 1,

    /// <summary>
    /// The <see cref="CosmosClient"/> is created using the <see cref="ICosmosOptions.ConnectionString"/>.
    /// </summary>
    ConnectionString = 2,
}
