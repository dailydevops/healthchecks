namespace NetEvolve.HealthChecks.Azure.EventHubs;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using global::Azure.Core;
using global::Azure.Identity;
using global::Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.DependencyInjection;

internal sealed class EventHubsClientFactory : IAsyncDisposable
{
    private static readonly EventHubProducerClientOptions _clientOptions = new EventHubProducerClientOptions();

    private readonly ConcurrentDictionary<string, EventHubProducerClient> _eventHubClients = new(
        StringComparer.OrdinalIgnoreCase
    );

    static EventHubsClientFactory() => _clientOptions.RetryOptions.MaximumRetries = 0;

    internal EventHubProducerClient GetClient(string name, EventHubsOptions options, IServiceProvider serviceProvider)
    {
        if (options.Mode == ClientCreationMode.ServiceProvider)
        {
            return serviceProvider.GetRequiredService<EventHubProducerClient>();
        }

        return _eventHubClients.GetOrAdd(name, _ => CreateClient(options, serviceProvider));
    }

    private static EventHubProducerClient CreateClient(EventHubsOptions options, IServiceProvider serviceProvider) =>
        options.Mode switch
        {
            ClientCreationMode.DefaultAzureCredentials => new EventHubProducerClient(
                options.FullyQualifiedNamespace,
                options.EventHubName,
                serviceProvider.GetService<TokenCredential>() ?? new DefaultAzureCredential(),
                _clientOptions
            ),
            ClientCreationMode.ConnectionString => new EventHubProducerClient(
                options.ConnectionString,
                options.EventHubName,
                _clientOptions
            ),
            _ => throw new UnreachableException($"Invalid client creation mode `{options.Mode}`."),
        };

    public async ValueTask DisposeAsync()
    {
        foreach (var client in _eventHubClients.Values)
        {
            await client.DisposeAsync().ConfigureAwait(false);
        }

        _eventHubClients.Clear();
    }
}
