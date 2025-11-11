namespace NetEvolve.HealthChecks.Azure.Kusto;

using System;

/// <summary>
/// Options for the <see cref="KustoAvailableHealthCheck"/>.
/// </summary>
public sealed record KustoAvailableOptions
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
    /// Gets or sets the cluster URI.
    /// </summary>
    public Uri? ClusterUri { get; set; }

    /// <summary>
    /// Gets or sets the database name to query.
    /// </summary>
    public string? DatabaseName { get; set; }
}
