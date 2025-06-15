namespace NetEvolve.HealthChecks.Elasticsearch;

using System;
using System.Collections.Concurrent;
using Elastic.Clients.Elasticsearch;
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
        ArgumentOutOfRangeException.ThrowIfNotEqual((int)options.Mode, (int)ElasticsearchClientCreationMode.Internal);
        ArgumentNullException.ThrowIfNullOrEmpty(options.ConnectionString);

        using var settings = new ElasticsearchClientSettings(new Uri(options.ConnectionString));
        _ = settings.ServerCertificateValidationCallback(CertificateValidations.AllowAll);

        if (options.Username is not null && options.Password is not null)
        {
            _ = settings.Authentication(new BasicAuthentication(options.Username, options.Password));
        }

        return new ElasticsearchClient(settings);
    }
}
