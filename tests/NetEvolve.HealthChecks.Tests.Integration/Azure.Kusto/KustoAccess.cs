namespace NetEvolve.HealthChecks.Tests.Integration.Azure.Kusto;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.Kusto;

public sealed class KustoAccess : IAsyncInitializer, IAsyncDisposable
{
    private readonly KustoContainer _container = new KustoBuilder().WithLogger(NullLogger.Instance).Build();

    public string ConnectionString => _container.GetConnectionString();

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);
}
