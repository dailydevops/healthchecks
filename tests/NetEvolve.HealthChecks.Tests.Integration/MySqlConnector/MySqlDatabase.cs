namespace NetEvolve.HealthChecks.Tests.Integration.MySqlConnector;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Testcontainers.MySql;
using Xunit;

public sealed class MySqlDatabase : IAsyncLifetime
{
    private readonly MySqlContainer _database = new MySqlBuilder().Build();

    public string ConnectionString => _database.GetConnectionString();

    public async Task DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
