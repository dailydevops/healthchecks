namespace NetEvolve.HealthChecks.Tests.Integration.Firebird;

using System.Threading.Tasks;
using Testcontainers.FirebirdSql;

public sealed class FirebirdDatabase : IAsyncLifetime
{
    private readonly FirebirdSqlContainer _database = new FirebirdSqlBuilder().Build();

    public string ConnectionString => _database.GetConnectionString();

    public async Task DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
