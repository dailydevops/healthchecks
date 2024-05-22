namespace NetEvolve.HealthChecks.Azure.Blobs;

using System;
using global::Azure.Storage.Blobs;

internal interface IBlobOptions
{
    Uri? ServiceUri { get; }

    string? ConnectionString { get; }

    string? AccountName { get; }

    string? AccountKey { get; }

    BlobClientCreationMode Mode { get; }

    Action<BlobClientOptions>? ConfigureClientOptions { get; }

    int Timeout { get; }
}
