namespace NetEvolve.HealthChecks.Azure.Blobs;

using System;
using global::Azure.Storage.Blobs;

/// <summary>
/// Options for the <see cref="BlobServiceAvailableHealthCheck"/>.
/// </summary>
public sealed class BlobServiceAvailableOptions : IBlobOptions
{
    /// <inheritdoc/>
    public string? ConnectionString { get; set; }

    /// <inheritdoc/>
    public ClientCreationMode Mode { get; set; }

    /// <summary>
    /// Gets or sets the timeout in milliseconds for executing the healthcheck.
    /// </summary>
    public int Timeout { get; set; } = 100;

    /// <inheritdoc/>
    public Uri? ServiceUri { get; set; }

    /// <inheritdoc/>
    public string? AccountName { get; set; }

    /// <inheritdoc/>
    public string? AccountKey { get; set; }

    /// <inheritdoc/>
    public Action<BlobClientOptions>? ConfigureClientOptions { get; set; }
}
