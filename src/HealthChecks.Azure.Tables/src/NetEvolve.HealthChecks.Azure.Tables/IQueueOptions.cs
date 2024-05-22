namespace NetEvolve.HealthChecks.Azure.Tables;

using System;
using global::Azure.Data.Tables;

internal interface ITableOptions
{
    Uri? ServiceUri { get; }

    string? ConnectionString { get; }

    string? AccountName { get; }

    string? AccountKey { get; }

    TableClientCreationMode Mode { get; }

    Action<TableClientOptions>? ConfigureClientOptions { get; }

    int Timeout { get; }
}
