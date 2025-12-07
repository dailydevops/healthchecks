namespace NetEvolve.HealthChecks.Tests.Integration.Garnet;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.Redis;

public sealed class GarnetContainer : IAsyncInitializer, IAsyncDisposable
{
    private readonly RedisContainer _database = new RedisBuilder()
        .WithImage("ghcr.io/microsoft/garnet")
        .WithLogger(NullLogger.Instance)
        .Build();

    public string Hostname => _database.Hostname;

    public int Port => _database.GetMappedPublicPort(6379);

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
