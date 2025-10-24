namespace NetEvolve.HealthChecks.Tests.Integration.Azure.Kusto;

using System;
using System.Threading.Tasks;
using global::Kusto.Data;
using global::Kusto.Data.Net.Client;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.Kusto;

public sealed class KustoAccess : IAsyncInitializer, IAsyncDisposable
{
    private readonly KustoContainer _container = new KustoBuilder().WithLogger(NullLogger.Instance).Build();

    public string ConnectionString => _container.GetConnectionString();

    public Uri ClusterUri { get; private set; } = default!;

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync()
    {
        await _container.StartAsync().ConfigureAwait(false);
        var connectionString = _container.GetConnectionString();
        ClusterUri = new Uri(connectionString.Split(';')[0].Replace("Data Source=", "", StringComparison.Ordinal));
    }
}
