namespace NetEvolve.HealthChecks.Tests.Integration.ClickHouse;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Testcontainers.ClickHouse;
using Xunit;

public sealed class ClickHouseDatabase : IAsyncLifetime
{
    private readonly ClickHouseContainer _database = new ClickHouseBuilder().Build();

    public string ConnectionString => _database.GetConnectionString();

    public async Task DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
