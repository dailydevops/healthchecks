namespace NetEvolve.HealthChecks.Meilisearch;

using System;
using System.Collections.Concurrent;
using Meilisearch;
using Microsoft.Extensions.DependencyInjection;

internal sealed class MeilisearchClientProvider
{
    private ConcurrentDictionary<string, global::Meilisearch.MeilisearchClient>? _meilisearchClients;

    internal global::Meilisearch.MeilisearchClient GetClient(
        string name,
        MeilisearchOptions options,
        IServiceProvider serviceProvider
    )
    {
        if (options.Mode == MeilisearchClientCreationMode.ServiceProvider)
        {
            return string.IsNullOrWhiteSpace(options.KeyedService)
                ? serviceProvider.GetRequiredService<global::Meilisearch.MeilisearchClient>()
                : serviceProvider.GetRequiredKeyedService<global::Meilisearch.MeilisearchClient>(options.KeyedService);
        }

        _meilisearchClients ??= new ConcurrentDictionary<string, global::Meilisearch.MeilisearchClient>(
            StringComparer.OrdinalIgnoreCase
        );

        return _meilisearchClients.GetOrAdd(name, _ => CreateClient(options));
    }

    internal static global::Meilisearch.MeilisearchClient CreateClient(MeilisearchOptions options)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(
            (int)options.Mode,
            (int)MeilisearchClientCreationMode.Internal
        );
        ArgumentException.ThrowIfNullOrEmpty(options.Host);

        return new global::Meilisearch.MeilisearchClient(options.Host, options.ApiKey);
    }
}
