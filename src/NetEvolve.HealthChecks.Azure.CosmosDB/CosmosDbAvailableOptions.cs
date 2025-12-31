namespace NetEvolve.HealthChecks.Azure.CosmosDB;

using System;

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
    /// Gets or sets the mode to create the client.
    /// </summary>
    public CosmosDbClientCreationMode? Mode { get; set; }
}
