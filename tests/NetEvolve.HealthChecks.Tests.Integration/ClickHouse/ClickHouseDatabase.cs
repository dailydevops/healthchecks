namespace NetEvolve.HealthChecks.Tests.Integration.ClickHouse;

using System.Threading.Tasks;
using Testcontainers.ClickHouse;

public sealed class ClickHouseDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly ClickHouseContainer _database = new ClickHouseBuilder().Build();

    public string ConnectionString => _database.GetConnectionString();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
