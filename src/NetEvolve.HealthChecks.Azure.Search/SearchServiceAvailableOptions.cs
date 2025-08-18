namespace NetEvolve.HealthChecks.Azure.Search;

using System;
using global::Azure.Search.Documents;

/// <summary>
/// Options for the <see cref="SearchServiceAvailableHealthCheck"/>.
/// </summary>
public sealed record SearchServiceAvailableOptions : ISearchOptions
{
    /// <summary>
    /// Gets or sets the connection string.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the mode to create the client.
    /// </summary>
    public SearchClientCreationMode? Mode { get; set; }

    /// <summary>
    /// Gets or sets the timeout in milliseconds for executing the healthcheck.
    /// </summary>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// Gets or sets the service uri.
    /// </summary>
    public Uri? ServiceUri { get; set; }

    /// <summary>
    /// Gets or sets the API key.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the lambda to configure the <see cref="SearchClientOptions"/>.
    /// </summary>
    public Action<SearchClientOptions>? ConfigureClientOptions { get; set; }
}