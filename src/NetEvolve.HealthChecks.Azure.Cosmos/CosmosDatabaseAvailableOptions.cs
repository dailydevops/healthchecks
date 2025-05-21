namespace NetEvolve.HealthChecks.Azure.Cosmos;

using System;
using Microsoft.Azure.Cosmos;

/// <summary>
/// Options for the <see cref="CosmosDatabaseAvailableHealthCheck"/>.
/// </summary>
public sealed record CosmosDatabaseAvailableOptions : ICosmosOptions
{
    /// <summary>
    /// Gets or sets the connection string.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the endpoint URI.
    /// </summary>
    public Uri? EndpointUri { get; set; }

    /// <summary>
    /// Gets or sets the primary key.
    /// </summary>
    public string? PrimaryKey { get; set; }

    /// <summary>
    /// Gets or sets the mode to create the client.
    /// </summary>
    public CosmosClientCreationMode Mode { get; set; }

    /// <summary>
    /// Gets or sets the timeout in milliseconds for executing the healthcheck.
    /// </summary>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// Gets or sets the name of the database to check.
    /// </summary>
    public string? DatabaseId { get; set; }

    /// <summary>
    /// Gets or sets the lambda to configure the <see cref="CosmosClientOptions"/>.
    /// </summary>
    public Action<CosmosClientOptions>? ConfigureClientOptions { get; set; }
}
