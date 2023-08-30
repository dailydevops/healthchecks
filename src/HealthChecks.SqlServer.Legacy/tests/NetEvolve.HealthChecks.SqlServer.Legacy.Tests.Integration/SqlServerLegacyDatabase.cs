namespace NetEvolve.HealthChecks.SqlServer.Legacy.Tests.Integration;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Testcontainers.MsSql;
using Xunit;

[ExcludeFromCodeCoverage]
public sealed class SqlServerLegacyDatabase : IAsyncLifetime
{
    private readonly MsSqlContainer _database = new MsSqlBuilder().Build();

    public string GetConnectionString() => _database.GetConnectionString();

    public async Task DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
