namespace NetEvolve.HealthChecks.Azure.Kusto;

using System;
using Kusto.Data.Common;

internal interface IKustoOptions
{
    string? ConnectionString { get; }

    Uri? ClusterUri { get; }

    string? DatabaseName { get; }

    KustoClientCreationMode? Mode { get; }

    Action<KustoConnectionStringBuilder>? ConfigureConnectionStringBuilder { get; }

    int Timeout { get; }
}
