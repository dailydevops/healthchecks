namespace NetEvolve.HealthChecks.Tests.Integration.MySql;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.MySql;

public sealed class MySqlDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly MySqlContainer _database = new MySqlBuilder(
        /*dockerimage*/"mysql:8.0.44"
    )
        .WithLogger(NullLogger.Instance)
        .Build();

    public string ConnectionString => _database.GetConnectionString();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
