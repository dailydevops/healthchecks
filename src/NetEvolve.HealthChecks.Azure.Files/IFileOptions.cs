namespace NetEvolve.HealthChecks.Azure.Files;

using System;
using global::Azure.Storage.Files.Shares;

internal interface IFileOptions
{
    Uri? ServiceUri { get; }

    string? ConnectionString { get; }

    string? AccountName { get; }

    string? AccountKey { get; }

    FileClientCreationMode? Mode { get; }

    Action<ShareClientOptions>? ConfigureClientOptions { get; }

    int Timeout { get; }
}