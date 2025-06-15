namespace NetEvolve.HealthChecks.Elasticsearch.Cluster;

using System;
using System.Collections.Concurrent;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.DependencyInjection;

internal class ElasticsearchClusterClientProvider
{
    private ConcurrentDictionary<string, ElasticsearchClient>? _clients;

    internal ElasticsearchClient GetClient(
        string name,
        ElasticsearchClusterOptions options,
        IServiceProvider serviceProvider
    )
    {
        if (options.Mode == ElasticsearchClusterClientCreationMode.ServiceProvider)
        {
            return string.IsNullOrWhiteSpace(options.KeyedService)
                ? serviceProvider.GetRequiredService<ElasticsearchClient>()
                : serviceProvider.GetRequiredKeyedService<ElasticsearchClient>(options.KeyedService);
        }

        _clients ??= new ConcurrentDictionary<string, ElasticsearchClient>(StringComparer.OrdinalIgnoreCase);

        return _clients.GetOrAdd(name, _ => CreateClient(options));
    }

    internal static ElasticsearchClient CreateClient(ElasticsearchClusterOptions options)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(
            (int)options.Mode,
            (int)ElasticsearchClusterClientCreationMode.Internal
        );

        if (options.ConnectionStrings?.Any() != true)
        {
            throw new ArgumentException("At least one connection string must be provided.");
        }

        var nodes = options.ConnectionStrings.Select(connectionString => new Uri(connectionString)).ToArray();

        using var pool = new StaticNodePool(nodes);

        using var settings = new ElasticsearchClientSettings(pool);
        _ = settings.ServerCertificateValidationCallback(CertificateValidations.AllowAll);

        if (options.Username is not null && options.Password is not null)
        {
            _ = settings.Authentication(new BasicAuthentication(options.Username, options.Password));
        }

        return new ElasticsearchClient(settings);
    }
}
