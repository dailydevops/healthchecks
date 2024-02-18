namespace NetEvolve.HealthChecks.Azure.Blobs;

using System;
using global::Azure.Storage.Blobs;

/// <summary>
/// Options for the <see cref="BlobContainerAvailableHealthCheck"/>.
/// </summary>
public sealed class BlobContainerAvailableOptions : IBlobOptions
{
    /// <inheritdoc/>
    public string? ConnectionString { get; set; }

    /// <inheritdoc/>
    public ClientCreationMode Mode { get; set; }

    public int Timeout { get; set; } = 100;

    /// <inheritdoc/>
    public string? ContainerName { get; set; }

    /// <inheritdoc/>
    public Uri? ServiceUri { get; set; }

    /// <inheritdoc/>
    public string? AccountName { get; set; }

    /// <inheritdoc/>
    public string? AccountKey { get; set; }

    /// <inheritdoc/>
    public Action<BlobClientOptions>? ConfigureClientOptions { get; set; }
}
