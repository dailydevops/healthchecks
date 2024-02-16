﻿namespace NetEvolve.HealthChecks.Azure.Blobs;

using System;
using System.Diagnostics.CodeAnalysis;
using global::Azure.Storage.Blobs;

/// <summary>
/// Options for the <see cref="BlobContainerAvailableHealthCheck"/>.
/// </summary>
public sealed class BlobContainerAvailableOptions
{
    public string? ConnectionString { get; set; }
    public ClientCreationMode Mode { get; set; }
    public int Timeout { get; set; } = 100;

    public string? ContainerName { get; set; }

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
