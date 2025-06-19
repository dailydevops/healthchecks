namespace NetEvolve.HealthChecks.Elasticsearch;

using System;
using System.Collections.Concurrent;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Nodes;
using Elastic.Transport;
using Microsoft.Extensions.DependencyInjection;

internal class ElasticsearchClientProvider
{
    private ConcurrentDictionary<string, ElasticsearchClient>? _clients;

    internal ElasticsearchClient GetClient(string name, ElasticsearchOptions options, IServiceProvider serviceProvider)
    {
        if (options.Mode == ElasticsearchClientCreationMode.ServiceProvider)
        {
            return string.IsNullOrWhiteSpace(options.KeyedService)
                ? serviceProvider.GetRequiredService<ElasticsearchClient>()
                : serviceProvider.GetRequiredKeyedService<ElasticsearchClient>(options.KeyedService);
        }

        _clients ??= new ConcurrentDictionary<string, ElasticsearchClient>(StringComparer.OrdinalIgnoreCase);

        return _clients.GetOrAdd(name, _ => CreateClient(options));
    }

    internal static ElasticsearchClient CreateClient(ElasticsearchOptions options)
    {
        var nodes = options.ConnectionStrings.Select(connectionString => new Uri(connectionString)).ToArray();

#pragma warning disable CA2000 // Dispose objects before losing scope
        var settings =
            options.ConnectionStrings.Count == 1
                ? new ElasticsearchClientSettings(nodes[0])
                : new ElasticsearchClientSettings(new StaticNodePool(nodes));
#pragma warning restore CA2000 // Dispose objects before losing scope

        _ = settings.ServerCertificateValidationCallback(CertificateValidations.AllowAll);

        if (options.Username is not null && options.Password is not null)
        {
            _ = settings.Authentication(new BasicAuthentication(options.Username, options.Password));
        }

        return new ElasticsearchClient(settings);
    }
}
