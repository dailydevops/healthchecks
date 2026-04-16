namespace NetEvolve.HealthChecks.Tests.Integration.Apache.RocketMQ;

using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging.Abstractions;

/// <summary>
/// Provides a Testcontainers-based RocketMQ broker for integration testing.
/// Uses the Apache RocketMQ docker image to spin up a name server and broker.
/// </summary>
public sealed class RocketMQContainer : IAsyncInitializer, IAsyncDisposable, IRocketMQAccessor
{
    private const int NameServerPort = 9876;
    private const int BrokerPort = 10911;

    private readonly IContainer _nameServer = new ContainerBuilder()
        .WithImage("apache/rocketmq:5.3.1")
        .WithCommand("sh", "mqnamesrv")
        .WithPortBinding(NameServerPort, true)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(NameServerPort))
        .WithLogger(NullLogger.Instance)
        .Build();

    public string Endpoint => $"{_nameServer.Hostname}:{_nameServer.GetMappedPublicPort(NameServerPort)}";

    public string Topic => "health-check-topic";

    public string? AccessKey => null;

    public string? AccessSecret => null;

    public async ValueTask DisposeAsync() => await _nameServer.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _nameServer.StartAsync().ConfigureAwait(false);
}
