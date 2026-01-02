namespace NetEvolve.HealthChecks.Tests.Integration.IbmMQ;

using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging.Abstractions;

public sealed class IbmMQContainer : IAsyncInitializer, IAsyncDisposable
{
    private readonly IContainer _container = new ContainerBuilder()
        .WithImage("icr.io/ibm-messaging/mq:9.4.1.0-r1")
        .WithPortBinding(1414, true)
        .WithPortBinding(9443, true)
        .WithEnvironment("LICENSE", "accept")
        .WithEnvironment("MQ_QMGR_NAME", "QM1")
        .WithEnvironment("MQ_APP_PASSWORD", "passw0rd")
        .WithLogger(NullLogger.Instance)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1414))
        .Build();

    public string QueueManagerName => "QM1";

    public string Host => _container.Hostname;

    public int Port => _container.GetMappedPublicPort(1414);

    public string Channel => "DEV.APP.SVRCONN";

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);
}
