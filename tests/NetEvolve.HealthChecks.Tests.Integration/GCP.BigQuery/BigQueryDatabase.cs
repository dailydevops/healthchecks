namespace NetEvolve.HealthChecks.Tests.Integration.GCP.BigQuery;

using System;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.BigQuery.V2;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.BigQuery;
using TUnit.Core.Interfaces;

public sealed class BigQueryDatabase : IAsyncInitializer, IAsyncDisposable
{
    public const string ProjectId = "test-project";

    private readonly BigQueryContainer _container = new BigQueryBuilder(
        /*dockerimage*/"ghcr.io/goccy/bigquery-emulator:0.4.4"
    )
        .WithProject(ProjectId)
        .WithLogger(NullLogger.Instance)
        .Build();

    public string Endpoint => _container.GetEmulatorEndpoint();

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync()
    {
        await _container.StartAsync().ConfigureAwait(false);

        var builder = new BigQueryClientBuilder
        {
            BaseUri = _container.GetEmulatorEndpoint(),
            ProjectId = ProjectId,
            Credential = GoogleCredential.FromAccessToken("fake-token"),
        };

        using var client = await builder.BuildAsync().ConfigureAwait(false);

        _ = await client.CreateDatasetAsync("test_dataset").ConfigureAwait(false);
    }
}
