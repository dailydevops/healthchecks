namespace NetEvolve.HealthChecks.Tests.Integration.Meilisearch;

using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging.Abstractions;

public sealed class MeilisearchContainer : IAsyncInitializer, IAsyncDisposable
{
    private const int DefaultPort = 7700;
    private readonly IContainer _database = new ContainerBuilder(
        /*dockerimage*/"getmeili/meilisearch:v1.31.0"
    )
        .WithPortBinding(DefaultPort, true)
        .WithEnvironment("MEILI_ENV", "development")
        .WithEnvironment("MEILI_NO_ANALYTICS", "true")
        .WithLogger(NullLogger.Instance)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(DefaultPort)))
        .Build();

    public string Hostname => _database.Hostname;

    public int Port => _database.GetMappedPublicPort(DefaultPort);

    public string Host => $"http://{Hostname}:{Port}";

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
