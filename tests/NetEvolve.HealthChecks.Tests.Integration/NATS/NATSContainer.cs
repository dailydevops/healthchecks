namespace NetEvolve.HealthChecks.Tests.Integration.NATS;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.Nats;

public sealed class NATSContainer : IAsyncInitializer, IAsyncDisposable
{
    private readonly NatsContainer _container = new NatsBuilder().WithLogger(NullLogger.Instance).Build();

    public string ConnectionString => _container.GetConnectionString();

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);
}
