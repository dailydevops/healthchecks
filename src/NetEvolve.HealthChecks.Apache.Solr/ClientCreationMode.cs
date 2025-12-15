namespace NetEvolve.HealthChecks.Apache.Solr;

/// <summary>
/// Defines how <see cref="SolrHealthCheck"/> obtains <see cref="SolrNet.ISolrBasicReadOnlyOperations{T}"/> clients.
/// </summary>
public enum ClientCreationMode
{
    /// <summary>
    /// Resolves clients from the dependency injection container.
    /// </summary>
    ServiceProvider = 0,

    /// <summary>
    /// Creates clients on demand using the provided base URL.
    /// </summary>
    Create = 1,
}
