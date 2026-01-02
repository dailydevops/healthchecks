namespace NetEvolve.HealthChecks.Tests.Integration.KurrentDb;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.KurrentDb;

public sealed class KurrentDbDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly KurrentDbContainer _database = new KurrentDbBuilder(
        /*dockerimage*/"kurrentplatform/kurrentdb:25.1"
    )
        .WithLogger(NullLogger.Instance)
        .Build();

    public string ConnectionString => _database.GetConnectionString();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
