namespace NetEvolve.HealthChecks.Tests.Integration.GCP.BigQuery;

using System;
using System.Threading.Tasks;
using Google.Cloud.BigQuery.V2;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.BigQuery;
using TUnit.Core.Interfaces;

public sealed class BigQueryDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly BigQueryContainer _container = new BigQueryBuilder().WithLogger(NullLogger.Instance).Build();

    private BigQueryClient? _client;

    public const string ProjectId = "test-project";

    public BigQueryClient Client => _client ?? throw new InvalidOperationException("Client not initialized");

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync()
    {
        await _container.StartAsync().ConfigureAwait(false);

        var builder = new BigQueryClientBuilder { BaseUri = _container.GetEmulatorEndpoint(), ProjectId = ProjectId };

        _client = await builder.BuildAsync().ConfigureAwait(false);
    }
}
