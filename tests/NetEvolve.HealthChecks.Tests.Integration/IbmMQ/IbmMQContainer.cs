namespace NetEvolve.HealthChecks.Tests.Integration.IbmMQ;

using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging.Abstractions;

public sealed class IbmMQContainer : IAsyncInitializer, IAsyncDisposable
{
    internal const string Password = "passw0rd";
    internal const string User = "app";
    internal const string QueueManager = "QM1";
    internal const string Channel = "DEV.APP.SVRCONN";
    internal const int PortNumber = 1414;

    private readonly IContainer _container = new ContainerBuilder(
        /*dockerimage*/"icr.io/ibm-messaging/mq:9.2.4.0-r1"
    )
        .WithPortBinding(PortNumber, true)
        .WithEnvironment("LICENSE", "accept")
        .WithEnvironment("MQ_QMGR_NAME", QueueManager)
        .WithEnvironment("MQ_APP_PASSWORD", Password)
        .WithLogger(NullLogger.Instance)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("chkmqready"))
        .Build();

    public string Host => $"localhost({Port})";

    public int Port => _container.GetMappedPublicPort(PortNumber);

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);
}
