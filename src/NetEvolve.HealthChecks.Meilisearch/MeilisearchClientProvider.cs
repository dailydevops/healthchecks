namespace NetEvolve.HealthChecks.Meilisearch;

using System;
using System.Collections.Concurrent;
using Meilisearch;
using Microsoft.Extensions.DependencyInjection;

internal sealed class MeilisearchClientProvider
{
    private ConcurrentDictionary<string, MeilisearchClient>? _meilisearchClients;

    internal MeilisearchClient GetClient(
        string name,
        MeilisearchOptions options,
        IServiceProvider serviceProvider
    )
    {
        if (options.Mode == MeilisearchClientCreationMode.ServiceProvider)
        {
            return string.IsNullOrWhiteSpace(options.KeyedService)
                ? serviceProvider.GetRequiredService<MeilisearchClient>()
                : serviceProvider.GetRequiredKeyedService<MeilisearchClient>(options.KeyedService);
        }

        _meilisearchClients ??= new ConcurrentDictionary<string, MeilisearchClient>(
            StringComparer.OrdinalIgnoreCase
        );

        return _meilisearchClients.GetOrAdd(name, _ => CreateClient(options));
    }

    internal static MeilisearchClient CreateClient(MeilisearchOptions options)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(
            (int)options.Mode,
            (int)MeilisearchClientCreationMode.Internal
        );
        ArgumentException.ThrowIfNullOrEmpty(options.Host);

        return new MeilisearchClient(options.Host, options.ApiKey);
    }
}
