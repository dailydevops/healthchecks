namespace NetEvolve.HealthChecks.Tests.Integration.Consul;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.Consul;
using TUnit.Core.Interfaces;

public sealed class ConsulDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly ConsulContainer _container = new ConsulBuilder().WithLogger(NullLogger.Instance).Build();

    public string HttpEndpoint => _container.GetBaseAddress();

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);
}
