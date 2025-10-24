namespace NetEvolve.HealthChecks.Azure.Kusto;

using System;
using global::Kusto.Data;

internal interface IKustoOptions
{
    string? ConnectionString { get; }

    Uri? ClusterUri { get; }

    string? DatabaseName { get; }

    KustoClientCreationMode? Mode { get; }

    Action<KustoConnectionStringBuilder>? ConfigureConnectionStringBuilder { get; }

    int Timeout { get; }
}
