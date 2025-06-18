namespace NetEvolve.HealthChecks.Tests.Integration.Firebird;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.FirebirdSql;

public sealed class FirebirdDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly FirebirdSqlContainer _database = new FirebirdSqlBuilder().WithLogger(NullLogger.Instance).Build();

    public string ConnectionString => _database.GetConnectionString();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
