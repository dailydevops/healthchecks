namespace NetEvolve.HealthChecks.Tests.Integration.SqlServer;

using System.Threading.Tasks;
using Testcontainers.MsSql;

public sealed class SqlServerDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly MsSqlContainer _database = new MsSqlBuilder().Build();

    public string ConnectionString => _database.GetConnectionString();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
