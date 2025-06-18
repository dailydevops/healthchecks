namespace NetEvolve.HealthChecks.Tests.Integration.Redis;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.Redis;

public sealed class RedisContainer : IAsyncInitializer, IAsyncDisposable
{
    private readonly Testcontainers.Redis.RedisContainer _database = new RedisBuilder()
        .WithLogger(NullLogger.Instance)
        .Build();

    public string GetConnectionString() => _database.GetConnectionString();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
