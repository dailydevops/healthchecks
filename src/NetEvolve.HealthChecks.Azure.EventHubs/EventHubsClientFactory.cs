namespace NetEvolve.HealthChecks.Azure.EventHubs;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using global::Azure.Core;
using global::Azure.Identity;
using global::Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.DependencyInjection;

internal sealed class EventHubsClientFactory : IDisposable
{
    private static readonly EventHubProducerClientOptions _clientOptions = new EventHubProducerClientOptions();

    private readonly ConcurrentDictionary<string, EventHubProducerClient> _eventHubClients = new(
        StringComparer.OrdinalIgnoreCase
    );

    private bool _disposedValue;

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

    [SuppressMessage(
        "Blocker Code Smell",
        "S2953:Methods named \"Dispose\" should implement \"IDisposable.Dispose\"",
        Justification = "As designed."
    )]
    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
#pragma warning disable VSTHRD101 // Avoid unsupported async delegates
                _ = Parallel.ForEach(_eventHubClients.Values, async client => await client.DisposeAsync().ConfigureAwait(false));
#pragma warning restore VSTHRD101 // Avoid unsupported async delegates
                _eventHubClients.Clear();
            }

            _disposedValue = true;
        }
    }

    void IDisposable.Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
