namespace NetEvolve.HealthChecks.Npgsql.Tests.Integration;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;
using Xunit;

[ExcludeFromCodeCoverage]
public sealed class NpgsqlDatabase : IAsyncLifetime
{
    private readonly PostgreSqlContainer _database = new PostgreSqlBuilder().Build();

    public string ConnectionString => _database.GetConnectionString();

    public async Task DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
