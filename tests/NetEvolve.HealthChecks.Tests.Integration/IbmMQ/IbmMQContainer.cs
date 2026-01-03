namespace NetEvolve.HealthChecks.Tests.Integration.IbmMQ;

using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging.Abstractions;

public sealed class IbmMQContainer : IAsyncInitializer, IAsyncDisposable
{
    private readonly IContainer _container = new ContainerBuilder(
        /*dockerimage*/"icr.io/ibm-messaging/mq:9.4.1.0-r1"
    )
        .WithPortBinding(1414, true)
        .WithPortBinding(9443, true)
        .WithEnvironment("LICENSE", "accept")
        .WithEnvironment("MQ_QMGR_NAME", "QM1")
        .WithEnvironment("MQ_APP_PASSWORD", "passw0rd")
        .WithLogger(NullLogger.Instance)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("chkmqready"))
        .Build();

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
    public string QueueManagerName => "QM1";
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static

    public string Host => _container.Hostname;

    public int Port => _container.GetMappedPublicPort(1414);

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
    public string Channel => "DEV.APP.SVRCONN";
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);
}
