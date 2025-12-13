namespace NetEvolve.HealthChecks.Meilisearch;

using Meilisearch;

/// <summary>
/// Options for <see cref="MeilisearchHealthCheck"/>
/// </summary>
public sealed record MeilisearchOptions
{
    /// <summary>
    /// Gets or sets the mode used to create/retrieve a client instance.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="MeilisearchClientCreationMode.ServiceProvider"/>.
    /// </remarks>
    public MeilisearchClientCreationMode Mode { get; set; } = MeilisearchClientCreationMode.ServiceProvider;

    /// <summary>
    /// Gets or sets the key used to resolve the <see cref="global::Meilisearch.MeilisearchClient"/> from the service provider.
    /// </summary>
    /// <remarks>
    /// When specified, the health check will resolve the <see cref="global::Meilisearch.MeilisearchClient"/> using <c>IServiceProvider.GetRequiredKeyedService</c>.
    /// <br/>
    /// When null or empty, the health check will resolve the <see cref="global::Meilisearch.MeilisearchClient"/> using <c>IServiceProvider.GetRequiredService</c>.
    /// <br/>
    /// This option is only used when <see cref="Mode"/> is set to <see cref="MeilisearchClientCreationMode.ServiceProvider"/>.
    /// </remarks>
    public string? KeyedService { get; set; }

    /// <summary>
    /// Gets or sets the host URL to the Meilisearch instance to check.
    /// </summary>
    /// <remarks>
    /// This option is only required when <see cref="Mode"/> is set to <see cref="MeilisearchClientCreationMode.Internal"/>.
    /// </remarks>
    public string? Host { get; set; }

    /// <summary>
    /// Gets or sets the API key for authenticating with the Meilisearch instance.
    /// </summary>
    /// <remarks>
    /// This option is only used when <see cref="Mode"/> is set to <see cref="MeilisearchClientCreationMode.Internal"/>.
    /// </remarks>
    public string? ApiKey { get; set; }

    /// <summary>
    /// The timeout to use when connecting and executing tasks against the Meilisearch instance.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>100 milliseconds</c>.
    /// <br/>
    /// Values below <see cref="Timeout.Infinite"/> (-1) are invalid.
    /// </remarks>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// The command to execute against the Meilisearch instance.
    /// Returns <see langword="true"/> if successful, <see langword="false"/> otherwise.
    /// </summary>
    /// <remarks>For internal use only.</remarks>
    public Func<global::Meilisearch.MeilisearchClient, CancellationToken, Task<bool>> CommandAsync
    {
        get;
        internal set;
    } = MeilisearchHealthCheck.DefaultCommandAsync;
}
