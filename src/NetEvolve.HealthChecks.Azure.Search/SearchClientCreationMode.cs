namespace NetEvolve.HealthChecks.Azure.Search;

/// <summary>
/// Specifies the mode to create the <see cref="global::Azure.Search.Documents.SearchClient"/> or <see cref="global::Azure.Search.Documents.Indexes.SearchIndexClient"/>.
/// </summary>
public enum SearchClientCreationMode
{
    /// <summary>
    /// Use the <see cref="global::Azure.Search.Documents.SearchClient"/> or <see cref="global::Azure.Search.Documents.Indexes.SearchIndexClient"/> from the <see cref="System.IServiceProvider"/>.
    /// </summary>
    ServiceProvider,

    /// <summary>
    /// Create the <see cref="global::Azure.Search.Documents.SearchClient"/> or <see cref="global::Azure.Search.Documents.Indexes.SearchIndexClient"/> with a connection string.
    /// </summary>
    ConnectionString,

    /// <summary>
    /// Create the <see cref="global::Azure.Search.Documents.SearchClient"/> or <see cref="global::Azure.Search.Documents.Indexes.SearchIndexClient"/> with default azure credentials.
    /// </summary>
    DefaultAzureCredentials,

    /// <summary>
    /// Create the <see cref="global::Azure.Search.Documents.SearchClient"/> or <see cref="global::Azure.Search.Documents.Indexes.SearchIndexClient"/> with an API key.
    /// </summary>
    ApiKey,
}
