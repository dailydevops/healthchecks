namespace NetEvolve.HealthChecks.Tests.Integration.GCP.BigQuery;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.BigQuery;
using TUnit.Core.Interfaces;

public sealed class BigQueryDatabase : IAsyncInitializer, IAsyncDisposable
{
    public const string ProjectId = "test-project";

    private readonly BigQueryContainer _container = new BigQueryBuilder()
        .WithProject(ProjectId)
        .WithLogger(NullLogger.Instance)
        .Build();

    public string Endpoint => _container.GetEmulatorEndpoint();

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);
}
