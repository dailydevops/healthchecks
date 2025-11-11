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

        // Set the emulator endpoint environment variable
        Environment.SetEnvironmentVariable("BIGQUERY_EMULATOR_HOST", _container.GetEmulatorEndpoint());

        // Create BigQuery client configured for emulator
        _client = await BigQueryClient.CreateAsync(ProjectId).ConfigureAwait(false);
    }
}
