namespace NetEvolve.HealthChecks.Tests.Integration.Apache.Pulsar;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.Pulsar;

public sealed class PulsarContainer : IAsyncInitializer, IAsyncDisposable
{
    private readonly Testcontainers.Pulsar.PulsarContainer _container = new PulsarBuilder(
        /*dockerimage*/"apachepulsar/pulsar:3.3.9"
    )
        .WithLogger(NullLogger.Instance)
        .Build();

    public Uri ServiceUrl => new Uri(_container.GetBrokerAddress());

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);
}
