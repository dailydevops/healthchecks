namespace NetEvolve.HealthChecks.Tests.Integration.SqlServer;

using System.Threading.Tasks;
using Testcontainers.MsSql;
using Xunit;

public sealed class SqlServerDatabase : IAsyncLifetime
{
    private readonly MsSqlContainer _database = new MsSqlBuilder().Build();

    public string ConnectionString => _database.GetConnectionString();

    public async Task DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
