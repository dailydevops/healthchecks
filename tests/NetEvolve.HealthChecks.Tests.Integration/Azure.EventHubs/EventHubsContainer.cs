namespace NetEvolve.HealthChecks.Tests.Integration.Azure.EventHubs;

using System;
using System.Threading.Tasks;

/// <summary>
/// Note: This is a placeholder container for EventHubs testing.
/// Since there's no EventHubs emulator available in test containers,
/// tests are skipped by default and would require actual Azure EventHubs connection.
/// </summary>
public sealed class EventHubsContainer : IAsyncInitializer, IAsyncDisposable
{
    public const string EventHubName = "test-eventhub";

    // This would be a real EventHubs connection string if testing with actual Azure resources
    public string ConnectionString => "Endpoint=sb://fake.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=fake-key";

    public async ValueTask DisposeAsync()
    {
        // No container to dispose
        await Task.CompletedTask;
    }

    public async Task InitializeAsync()
    {
        // No container to start
        await Task.CompletedTask;
    }
}