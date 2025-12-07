namespace NetEvolve.HealthChecks.Tests.Integration.Garnet;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.Redis;

public sealed class GarnetContainer : IAsyncInitializer, IAsyncDisposable
{
    private readonly Testcontainers.Redis.RedisContainer _database = new RedisBuilder()
        .WithImage("ghcr.io/microsoft/garnet")
        .WithLogger(NullLogger.Instance)
        .Build();

    public string GetConnectionString() => _database.GetConnectionString();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
