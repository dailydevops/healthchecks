namespace NetEvolve.HealthChecks.OpenSearch;

using System;
using global::OpenSearch.Client;

/// <summary>
/// Describes the mode used to create the <see cref="OpenSearchClient"/>.
/// </summary>
public enum OpenSearchClientCreationMode
{
    /// <summary>
    /// The <see cref="OpenSearchClient"/> preregistered instance is retrieved from the <see cref="IServiceProvider"/>.
    /// </summary>
    /// <remarks>
    /// This is the default mode.
    /// </remarks>
    ServiceProvider = 0,

    /// <summary>
    /// The <see cref="OpenSearchClient"/> instance is created using the <see cref="OpenSearchOptions.ConnectionStrings"/>,
    /// the <see cref="OpenSearchOptions.Username"/> and the <see cref="OpenSearchOptions.Password"/>.
    /// </summary>
    UsernameAndPassword = 1,
}
