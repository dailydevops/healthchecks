namespace NetEvolve.HealthChecks.Azure.Blobs;

using global::Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Options for the <see cref="BlobContainerAvailableHealthCheck"/>.
/// </summary>
public sealed class BlobContainerAvailableOptions
{
    public string? ConnectionString { get; set; }
    public string ContainerName { get; set; } = default!;
    public ClientCreationMode Mode { get; set; }
    public int Timeout { get; set; } = 100;

    public Uri? ServiceUri { get; set; }
    public string? AccountName { get; set; }
    public string? AccountKey { get; set; }

    [SuppressMessage(
        "Design",
        "CA1044:Properties should not be write only",
        Justification = "As designed."
    )]
    public Action<BlobClientOptions>? ConfigureClientOptions { internal get; set; }
}
