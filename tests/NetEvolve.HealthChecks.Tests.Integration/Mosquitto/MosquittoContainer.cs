namespace NetEvolve.HealthChecks.Tests.Integration.Mosquitto;

using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging.Abstractions;

public sealed class MosquittoContainer : IAsyncInitializer, IAsyncDisposable
{
    private readonly IContainer _container = new ContainerBuilder()
        .WithImage("eclipse-mosquitto:2.0.20")
        .WithPortBinding(1883, true)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("mosquitto version"))
        .WithLogger(NullLogger.Instance)
        .Build();

    public string Host => _container.Hostname;

    public ushort Port => _container.GetMappedPublicPort(1883);

    public string ConnectionString => $"mqtt://{Host}:{Port}";

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);
}
