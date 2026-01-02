namespace NetEvolve.HealthChecks.Tests.Integration.MariaDb;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.MariaDb;

public sealed class MariaDbDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly MariaDbContainer _database = new MariaDbBuilder(
        /*dockerimage*/"mariadb:10.11.15"
    )
        .WithLogger(NullLogger.Instance)
        .Build();

    public string ConnectionString => _database.GetConnectionString();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
