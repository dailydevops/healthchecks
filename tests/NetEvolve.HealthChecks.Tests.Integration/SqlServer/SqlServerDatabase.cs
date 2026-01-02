namespace NetEvolve.HealthChecks.Tests.Integration.SqlServer;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.MsSql;

public sealed class SqlServerDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly MsSqlContainer _database = new MsSqlBuilder(
        /*dockerimage*/"mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04"
    )
        .WithLogger(NullLogger.Instance)
        .Build();

    public string ConnectionString => _database.GetConnectionString();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
