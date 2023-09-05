namespace NetEvolve.HealthChecks.MySql.Tests.Integration;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Testcontainers.MySql;
using Xunit;

[ExcludeFromCodeCoverage]
public sealed class MySqlDatabase : IAsyncLifetime
{
    private readonly MySqlContainer _database = new MySqlBuilder().Build();

    public string GetConnectionString() => _database.GetConnectionString();

    public async Task DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
