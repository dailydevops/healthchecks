namespace NetEvolve.HealthChecks.Azure.Search;

using System;
using global::Azure.Search.Documents;

internal interface ISearchOptions
{
    string? KeyedService { get; }

    Uri? ServiceUri { get; }

    string? ApiKey { get; }

    SearchIndexClientCreationMode? Mode { get; }

    Action<SearchClientOptions>? ConfigureClientOptions { get; }

    int Timeout { get; }
}
