namespace NetEvolve.HealthChecks.Apache.Solr;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Options for <see cref="SolrHealthCheck"/>.
/// </summary>
public sealed record SolrOptions
{
    /// <summary>
    /// Gets or sets how the Solr client is created (resolved from DI or created on demand).
    /// </summary>
    public ClientCreationMode CreationMode { get; set; }

    /// <summary>
    /// Gets or sets the base URL of the Solr server.
    /// </summary>
    [SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "As designed.")]
    public string BaseUrl { get; set; } = default!;

    /// <summary>
    /// Gets or sets the timeout in milliseconds for executing the healthcheck.
    /// </summary>
    public int Timeout { get; set; } = 100;
}
