namespace NetEvolve.HealthChecks.Tests.Integration.Redpanda;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.Redpanda;

public sealed class RedpandaDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly RedpandaContainer _database = new RedpandaBuilder(
        /*dockerimage*/"docker.redpanda.com/redpandadata/redpanda:v22.2.13"
    )
        .WithLogger(NullLogger.Instance)
        .Build();

    public string BootstrapAddress => _database.GetBootstrapAddress();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
