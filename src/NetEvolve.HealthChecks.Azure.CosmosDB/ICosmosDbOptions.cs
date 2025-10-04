namespace NetEvolve.HealthChecks.Azure.CosmosDB;

using System;
using Microsoft.Azure.Cosmos;

/// <summary>
/// Common interface for CosmosDB options.
/// </summary>
public interface ICosmosDbOptions
{
    /// <summary>
    /// Gets or sets the connection string.
    /// </summary>
    string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the mode to create the client.
    /// </summary>
    CosmosDbClientCreationMode? Mode { get; set; }

    /// <summary>
    /// Gets or sets the service endpoint.
    /// </summary>
    string? ServiceEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the account key.
    /// </summary>
    string? AccountKey { get; set; }

    /// <summary>
    /// Gets or sets the lambda to configure the <see cref="CosmosClientOptions"/>.
    /// </summary>
    Action<CosmosClientOptions>? ConfigureClientOptions { get; set; }

    /// <summary>
    /// The timeout to use when connecting and executing tasks against the database.
    /// </summary>
    int Timeout { get; set; }
}
