namespace NetEvolve.HealthChecks.Tests.Integration.CockroachDb;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.CockroachDb;

public sealed class CockroachDbDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly CockroachDbContainer _database = new CockroachDbBuilder(
        /*dockerimage*/"cockroachdb/cockroach:v23.2.28"
    )
        .WithLogger(NullLogger.Instance)
        .Build();

    public string ConnectionString => _database.GetConnectionString();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
