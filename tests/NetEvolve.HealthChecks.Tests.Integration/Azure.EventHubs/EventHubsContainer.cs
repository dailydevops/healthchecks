namespace NetEvolve.HealthChecks.Tests.Integration.Azure.EventHubs;

using System;
using System.Threading.Tasks;
using Testcontainers.EventHubs;

/// <summary>
/// Note: This is a placeholder container for EventHubs testing.
/// Since there's no EventHubs emulator available in test containers,
/// tests are skipped by default and would require actual Azure EventHubs connection.
/// </summary>
public sealed class EventHubsContainer : IAsyncInitializer, IAsyncDisposable
{
    public const string EventHubName = "test-eventhub";
    private readonly Testcontainers.EventHubs.EventHubsContainer _container = new EventHubsBuilder().Build();

    public string ConnectionString => _container.GetConnectionString();

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync()
    {
        await _container.StartAsync().ConfigureAwait(false);
    }
}
