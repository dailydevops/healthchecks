namespace NetEvolve.HealthChecks.Tests.Integration.Apache.RocketMQ;

using System;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using Microsoft.Extensions.Logging.Abstractions;

/// <summary>
/// Provides a Testcontainers-based RocketMQ broker for integration testing.
/// Uses the Apache RocketMQ docker image to spin up a name server, a broker and a proxy.
/// </summary>
/// <remarks>
/// The .NET client is a gRPC client, which talks to the RocketMQ proxy (not the name server), so the
/// proxy container's gRPC port is exposed as the health check <see cref="Endpoint"/>.
/// </remarks>
public sealed class RocketMQContainer : IAsyncInitializer, IAsyncDisposable, IRocketMQAccessor
{
    private const string ImageName = "apache/rocketmq:5.3.1";
    private const string NameServerAlias = "namesrv";
    private const string BrokerAlias = "broker";
    private const string ProxyAlias = "proxy";
    private const int NameServerPort = 9876;
    private const int BrokerPort = 10911;
    private const int ProxyGrpcPort = 8081;
    private const string ClusterName = "DefaultCluster";
    private const string TopicName = "health-check-topic";

    // The proxy's gRPC port accepts TCP connections slightly before its TLS/gRPC listener is
    // actually ready to serve requests (a JVM/Netty warm-up race), so the first real client
    // connection right after the port opens can be reset mid-handshake. The RocketMQ client has
    // no retry for this, so give the listener a moment to settle before running any test against it.
    private static readonly TimeSpan ProxySettleDelay = TimeSpan.FromSeconds(10);

    private readonly INetwork _network = new NetworkBuilder().Build();

    private readonly IContainer _nameServer;
    private readonly IContainer _broker;
    private readonly IContainer _proxy;

    public RocketMQContainer()
    {
        _nameServer = new ContainerBuilder(ImageName)
            .WithNetwork(_network)
            .WithNetworkAliases(NameServerAlias)
            .WithCommand("sh", "mqnamesrv")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(NameServerPort))
            .WithLogger(NullLogger.Instance)
            .Build();

        _broker = new ContainerBuilder(ImageName)
            .WithNetwork(_network)
            .WithNetworkAliases(BrokerAlias)
            .WithEnvironment("NAMESRV_ADDR", $"{NameServerAlias}:{NameServerPort}")
            .WithCommand("sh", "mqbroker")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(BrokerPort))
            .WithLogger(NullLogger.Instance)
            .Build();

        _proxy = new ContainerBuilder(ImageName)
            .WithNetwork(_network)
            .WithNetworkAliases(ProxyAlias)
            .WithEnvironment("NAMESRV_ADDR", $"{NameServerAlias}:{NameServerPort}")
            .WithPortBinding(ProxyGrpcPort, true)
            .WithCommand("sh", "mqproxy")
            // The proxy resolves the broker's route info from the name server on its own client-side
            // cache, which only refreshes on a fixed interval. Starting the proxy can therefore race
            // the broker's registration and crash on its first attempt (fails to create its internal
            // system topics); let Docker restart it rather than failing the test.
            .WithCreateParameterModifier(parameters =>
            {
                parameters.HostConfig ??= new HostConfig();
                parameters.HostConfig.RestartPolicy = new RestartPolicy
                {
                    Name = RestartPolicyKind.OnFailure,
                    MaximumRetryCount = 10,
                };
            })
            .WithWaitStrategy(
                Wait.ForUnixContainer()
                    .UntilExternalTcpPortIsAvailable(
                        ProxyGrpcPort,
                        strategy => strategy.WithRetries(60).WithInterval(TimeSpan.FromSeconds(2))
                    )
            )
            .WithLogger(NullLogger.Instance)
            .Build();
    }

    public string Endpoint => $"{_proxy.Hostname}:{_proxy.GetMappedPublicPort(ProxyGrpcPort)}";

    public string Topic => TopicName;

    public string? AccessKey => null;

    public string? AccessSecret => null;

    public async Task InitializeAsync()
    {
        await _network.CreateAsync().ConfigureAwait(false);
        await _nameServer.StartAsync().ConfigureAwait(false);
        await _broker.StartAsync().ConfigureAwait(false);

        // The gRPC proxy resolves a topic's route from the name server before forwarding a
        // publish request; it does not trigger the broker's auto-create-topic behavior the way
        // publishing directly through the native remoting protocol would. Without this, every
        // send fails with "No topic route info in name server for the topic".
        var updateTopicResult = await _broker
            .ExecAsync([
                "sh",
                "mqadmin",
                "updateTopic",
                "-n",
                $"{NameServerAlias}:{NameServerPort}",
                "-c",
                ClusterName,
                "-t",
                TopicName,
            ])
            .ConfigureAwait(false);

        if (updateTopicResult.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"Failed to create RocketMQ topic '{TopicName}'. Stdout: {updateTopicResult.Stdout} Stderr: {updateTopicResult.Stderr}"
            );
        }

        await _proxy.StartAsync().ConfigureAwait(false);
        await Task.Delay(ProxySettleDelay).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await _proxy.DisposeAsync().ConfigureAwait(false);
        await _broker.DisposeAsync().ConfigureAwait(false);
        await _nameServer.DisposeAsync().ConfigureAwait(false);
        await _network.DisposeAsync().ConfigureAwait(false);
    }
}
