namespace NetEvolve.HealthChecks.Tests.Integration.EventStoreDb;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.EventStoreDb;

public sealed class EventStoreDbDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly EventStoreDbContainer _database = new EventStoreDbBuilder()
        .WithLogger(NullLogger.Instance)
        .Build();

    public string ConnectionString => _database.GetConnectionString();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
