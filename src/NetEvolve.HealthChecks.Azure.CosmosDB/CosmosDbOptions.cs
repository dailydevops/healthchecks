namespace NetEvolve.HealthChecks.Azure.CosmosDB;

using System;
using Microsoft.Azure.Cosmos;

/// <summary>
/// Options for the <see cref="CosmosDbHealthCheck"/>.
/// </summary>
public sealed record CosmosDbOptions : ICosmosDbOptions
{
    /// <summary>
    /// Gets or sets the connection string.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the mode to create the client.
    /// </summary>
    public CosmosDbClientCreationMode? Mode { get; set; }

    /// <summary>
    /// Gets or sets the service endpoint.
    /// </summary>
    public string? ServiceEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the account key.
    /// </summary>
    public string? AccountKey { get; set; }

    /// <summary>
    /// Gets or sets the lambda to configure the <see cref="CosmosClientOptions"/>.
    /// </summary>
    public Action<CosmosClientOptions>? ConfigureClientOptions { get; set; }

    /// <summary>
    /// The timeout to use when connecting and executing tasks against the database.
    /// </summary>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// Gets or sets the database name to check.
    /// </summary>
    public string? DatabaseName { get; set; }

    /// <summary>
    /// Gets or sets the container name to check.
    /// </summary>
    public string? ContainerName { get; set; }
}
