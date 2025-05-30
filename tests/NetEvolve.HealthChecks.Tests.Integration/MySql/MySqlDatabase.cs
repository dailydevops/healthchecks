namespace NetEvolve.HealthChecks.Tests.Integration.MySql;

using System.Threading.Tasks;
using Testcontainers.MySql;

public sealed class MySqlDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly MySqlContainer _database = new MySqlBuilder().Build();

    public string ConnectionString => _database.GetConnectionString();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
