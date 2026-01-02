namespace NetEvolve.HealthChecks.Tests.Integration.Garnet;

using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging.Abstractions;

public sealed class GarnetContainer : IAsyncInitializer, IAsyncDisposable
{
    private const int DefaultPort = 6379;
    private readonly IContainer _database = new ContainerBuilder(
        /*dockerimage*/"ghcr.io/microsoft/garnet:latest"
    )
        .WithPortBinding(DefaultPort, true)
        .WithLogger(NullLogger.Instance)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(DefaultPort))
        .Build();

    public string Hostname => _database.Hostname;

    public int Port => _database.GetMappedPublicPort(DefaultPort);

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
