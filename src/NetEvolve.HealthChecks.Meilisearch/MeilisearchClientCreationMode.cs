namespace NetEvolve.HealthChecks.Meilisearch;

using Meilisearch;

/// <summary>
/// Describes the mode used to create the <see cref="global::Meilisearch.MeilisearchClient"/>.
/// </summary>
public enum MeilisearchClientCreationMode
{
    /// <summary>
    /// The <see cref="global::Meilisearch.MeilisearchClient"/> preregistered instance is retrieved from the <see cref="IServiceProvider"/>.
    /// </summary>
    /// <remarks>
    /// This is the default mode.
    /// </remarks>
    ServiceProvider = 0,

    /// <summary>
    /// The <see cref="global::Meilisearch.MeilisearchClient"/> instance is created using the <see cref="MeilisearchOptions.Host"/> and
    /// the <see cref="MeilisearchOptions.ApiKey"/>.
    /// </summary>
    Internal = 1,
}
