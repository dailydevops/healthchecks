namespace NetEvolve.HealthChecks.ArangoDb;

using System;
using System.Collections.Concurrent;
using ArangoDBNetStandard;
using ArangoDBNetStandard.Transport.Http;
using Microsoft.Extensions.DependencyInjection;

internal sealed class ArangoDbClientProvider
{
    private ConcurrentDictionary<string, ArangoDBClient>? _arangoDbClients;

    internal ArangoDBClient GetClient(ArangoDbOptions options, IServiceProvider serviceProvider)
    {
        if (options.Mode == ArangoDbClientCreationMode.ServiceProvider)
        {
            return string.IsNullOrWhiteSpace(options.KeyedService)
                ? serviceProvider.GetRequiredService<ArangoDBClient>()
                : serviceProvider.GetRequiredKeyedService<ArangoDBClient>(options.KeyedService);
        }

        _arangoDbClients ??= new ConcurrentDictionary<string, ArangoDBClient>(StringComparer.OrdinalIgnoreCase);

        var clientKey =
            options.Username is not null && options.Password is not null
                ? $"{options.TransportAddress}:{options.Username}-{options.Password}"
                : $"{options.TransportAddress}";

        return _arangoDbClients.GetOrAdd(clientKey, _ => CreateClient(options));
    }

    internal static ArangoDBClient CreateClient(ArangoDbOptions options)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual((int)options.Mode, (int)ArangoDbClientCreationMode.Internal);
        ArgumentNullException.ThrowIfNullOrEmpty(options.TransportAddress);

        if (options.Password is not null)
        {
            ArgumentNullException.ThrowIfNull(options.Username);
        }

        if (options.Username is not null)
        {
            ArgumentNullException.ThrowIfNull(options.Password);
        }

        var addressUri = new Uri(options.TransportAddress);

        var transport =
            options.Password is not null && options.Username is not null
                ? HttpApiTransport.UsingBasicAuth(addressUri, options.Username, options.Password)
                : HttpApiTransport.UsingNoAuth(addressUri);

        return new ArangoDBClient(transport);
    }
}
