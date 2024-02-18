namespace NetEvolve.HealthChecks.Azure.Blobs;

using System;
using global::Azure.Storage.Blobs;

internal interface IBlobOptions
{
    Uri? ServiceUri { get; }

    string? ConnectionString { get; }

    string? AccountName { get; }

    string? AccountKey { get; }

    ClientCreationMode Mode { get; }

    Action<BlobClientOptions>? ConfigureClientOptions { get; set; }
}
