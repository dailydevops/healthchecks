namespace NetEvolve.HealthChecks.Azure.CosmosDB;

using System;
using Microsoft.Azure.Cosmos;

/// <summary>
/// Options for <see cref="CosmosDbAvailableHealthCheck"/>.
/// </summary>
public sealed record CosmosDbAvailableOptions
{
    /// <summary>
    /// Gets or sets the connection string.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the timeout in milliseconds for executing the healthcheck.
    /// </summary>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// Gets or sets the account endpoint.
    /// </summary>
    public Uri? AccountEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the account key.
    /// </summary>
    public string? AccountKey { get; set; }

    /// <summary>
    /// Gets or sets the database id to check.
    /// </summary>
    public string? DatabaseId { get; set; }

    /// <summary>
    /// Gets or sets the key used to resolve the <c>CosmosClient</c> from the service provider.
    /// </summary>
    /// <remarks>
    /// When specified, the health check will resolve the <c>CosmosClient</c> using <c>IServiceProvider.GetRequiredKeyedService</c>.
    /// When null or empty, the health check will resolve the <c>CosmosClient</c> using <c>IServiceProvider.GetRequiredService</c>.
    /// </remarks>
    public string? KeyedService { get; set; }

    /// <summary>
    /// Gets or sets the mode to create the client.
    /// </summary>
    public CosmosDbClientCreationMode? Mode { get; set; }

    /// <summary>
    /// Gets or sets the action to configure the <see cref="CosmosClientOptions"/> used to create the <see cref="CosmosClient"/>.
    /// </summary>
    public Action<CosmosClientOptions>? ClientConfiguration { get; internal set; }
}
