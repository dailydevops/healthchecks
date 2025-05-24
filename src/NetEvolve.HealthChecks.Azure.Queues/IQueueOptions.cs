namespace NetEvolve.HealthChecks.Azure.Queues;

using System;
using global::Azure.Storage.Queues;

internal interface IQueueOptions
{
    Uri? ServiceUri { get; }

    string? ConnectionString { get; }

    string? AccountName { get; }

    string? AccountKey { get; }

    QueueClientCreationMode? Mode { get; }

    Action<QueueClientOptions>? ConfigureClientOptions { get; }

    int Timeout { get; }
}
