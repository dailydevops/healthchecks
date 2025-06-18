namespace NetEvolve.HealthChecks.Tests.Integration.ClickHouse;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.ClickHouse;

public sealed class ClickHouseDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly ClickHouseContainer _database = new ClickHouseBuilder().WithLogger(NullLogger.Instance).Build();

    public string ConnectionString => _database.GetConnectionString();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
