namespace NetEvolve.HealthChecks.Azure.EventHubs;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using global::Azure.Core;
using global::Azure.Identity;
using global::Azure.Messaging.EventHubs;
using global::Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.DependencyInjection;

internal sealed class EventHubsClientFactory
{
    private static readonly EventHubProducerClientOptions _clientOptions = new EventHubProducerClientOptions();

    private readonly ConcurrentDictionary<string, EventHubProducerClient> _eventHubClients = new(
        StringComparer.OrdinalIgnoreCase
    );

    static EventHubsClientFactory()
    {
        _clientOptions.RetryOptions.MaximumRetries = 0;
    }

    internal EventHubProducerClient GetClient<TOptions>(string name, TOptions options, IServiceProvider serviceProvider)
        where TOptions : EventHubsOptionsBase
    {
        if (options.Mode == ClientCreationMode.ServiceProvider)
        {
            return serviceProvider.GetRequiredService<EventHubProducerClient>();
        }

        return _eventHubClients.GetOrAdd(name, _ => CreateClient(options, serviceProvider));
    }

    private static EventHubProducerClient CreateClient<TOptions>(TOptions options, IServiceProvider serviceProvider)
        where TOptions : EventHubsOptionsBase
    {
        return options.Mode switch
        {
            ClientCreationMode.DefaultAzureCredentials when options is EventHubOptions ehOptions => 
                new EventHubProducerClient(
                    options.FullyQualifiedNamespace,
                    ehOptions.EventHubName,
                    serviceProvider.GetService<TokenCredential>() ?? new DefaultAzureCredential(),
                    _clientOptions
                ),
            ClientCreationMode.ConnectionString when options is EventHubOptions ehOptions => 
                new EventHubProducerClient(options.ConnectionString, ehOptions.EventHubName, _clientOptions),
            _ => throw new UnreachableException($"Invalid client creation mode `{options.Mode}`."),
        };
    }
}