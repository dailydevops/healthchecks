namespace NetEvolve.HealthChecks.Azure.Blob;

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

    [SuppressMessage(
        "Design",
        "CA1056:URI-like properties should not be strings",
        Justification = "As designed."
    )]
    public string? ServiceUri { get; set; }
    public string? AccountName { get; set; }
    public string? SharedAccessToken { get; set; }

    [SuppressMessage(
        "Design",
        "CA1044:Properties should not be write only",
        Justification = "As designed."
    )]
    public Action<BlobClientOptions>? ConfigureClientOptions { internal get; set; }
}
