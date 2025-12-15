namespace NetEvolve.HealthChecks.Apache.Solr;

using System;

/// <summary>
/// Options for <see cref="SolrHealthCheck"/>.
/// </summary>
public sealed record SolrOptions
{
    /// <summary>
    /// Gets or sets the base URL of the Solr server.
    /// </summary>
    public Uri BaseUrl { get; set; } = default!;

    /// <summary>
    /// Gets or sets the core or collection name to check.
    /// </summary>
    public string Core { get; set; } = default!;

    /// <summary>
    /// Gets or sets the timeout in milliseconds for executing the healthcheck.
    /// </summary>
    public int Timeout { get; set; } = 100;
}
