namespace NetEvolve.HealthChecks.Azure.Search;

using System;
using global::Azure.Search.Documents;

/// <summary>
/// Options for the <see cref="SearchIndexAvailableHealthCheck"/>.
/// </summary>
public sealed record SearchIndexAvailableOptions : ISearchOptions
{
    /// <summary>
    /// Gets or sets the key used to resolve the <c>SearchClient</c> from the service provider.
    /// </summary>
    /// <remarks>
    /// When specified, the health check will resolve the <c>SearchClient</c> using <c>IServiceProvider.GetRequiredKeyedService</c>.
    /// When null or empty, the health check will resolve the <c>SearchClient</c> using <c>IServiceProvider.GetRequiredService</c>.
    /// </remarks>
    public string? KeyedService { get; set; }

    /// <summary>
    /// Gets or sets the service uri.
    /// </summary>
    public Uri? ServiceUri { get; set; }

    /// <summary>
    /// Gets or sets the api key.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the mode to create the client.
    /// </summary>
    public SearchIndexClientCreationMode? Mode { get; set; }

    /// <summary>
    /// Gets or sets the timeout in milliseconds for executing the healthcheck.
    /// </summary>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// Gets or sets the lambda to configure the <see cref="SearchClientOptions"/>.
    /// </summary>
    public Action<SearchClientOptions>? ConfigureClientOptions { get; set; }

    /// <summary>
    /// Gets or sets the index name.
    /// </summary>
    public string? IndexName { get; set; }
}
