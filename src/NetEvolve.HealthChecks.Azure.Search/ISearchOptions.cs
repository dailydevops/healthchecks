namespace NetEvolve.HealthChecks.Azure.Search;

using System;
using global::Azure.Search.Documents;

internal interface ISearchOptions
{
    Uri? ServiceUri { get; }

    string? ConnectionString { get; }

    string? ApiKey { get; }

    SearchClientCreationMode? Mode { get; }

    Action<SearchClientOptions>? ConfigureClientOptions { get; }

    int Timeout { get; }
}
