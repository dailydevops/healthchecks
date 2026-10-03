namespace NetEvolve.HealthChecks.Tests.Integration.GCP.BigQuery;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.BigQuery.V2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.GCP.BigQuery;

[TestGroup($"GCP.{nameof(BigQuery)}")]
[TestGroup("Z04TestGroup")]
[ClassDataSource<BigQueryDatabase>(Shared = SharedType.PerClass)]
public sealed class BigQueryHealthCheckTests : HealthCheckTestBase
{
    private readonly BigQueryDatabase _database;

    public BigQueryHealthCheckTests(BigQueryDatabase database) => _database = database;

    [Test]
    public async Task AddBigQuery_UseOptions_Healthy(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var client = await CreateClientAsync(_database.Endpoint, BigQueryDatabase.ProjectId, cancellationToken);

        await RunAndVerify(
            healthChecks => healthChecks.AddBigQuery("TestContainerHealthy", options => options.Timeout = 10000),
            HealthStatus.Healthy,
            serviceBuilder: services => _ = services.AddSingleton(_ => client),
            cancellationToken: cancellationToken
        );
    }

    [Test]
    public async Task AddBigQuery_UseOptions_Degraded(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var client = await CreateClientAsync(_database.Endpoint, BigQueryDatabase.ProjectId, cancellationToken);

        await RunAndVerify(
            healthChecks => healthChecks.AddBigQuery("TestContainerDegraded", options => options.Timeout = 0),
            HealthStatus.Degraded,
            serviceBuilder: services => _ = services.AddSingleton(_ => client),
            cancellationToken: cancellationToken
        );
    }

    [Test]
    public async Task AddBigQuery_UseOptionsWithKeyedService_Healthy(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var client = await CreateClientAsync(_database.Endpoint, BigQueryDatabase.ProjectId, cancellationToken);

        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddBigQuery(
                    "TestContainerKeyedServiceHealthy",
                    options =>
                    {
                        options.Timeout = 10000;
                        options.KeyedService = "bigquery";
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services => _ = services.AddKeyedSingleton("bigquery", (_, _) => client),
            cancellationToken: cancellationToken
        );
    }

    [Test]
    public async Task AddBigQuery_UseConfiguration_Healthy(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var client = await CreateClientAsync(_database.Endpoint, BigQueryDatabase.ProjectId, cancellationToken);

        await RunAndVerify(
            healthChecks => healthChecks.AddBigQuery("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:GCP:BigQuery:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => _ = services.AddSingleton(_ => client),
            cancellationToken: cancellationToken
        );
    }

    [Test]
    public async Task AddBigQuery_UseConfiguration_Degraded(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var client = await CreateClientAsync(_database.Endpoint, BigQueryDatabase.ProjectId, cancellationToken);

        await RunAndVerify(
            healthChecks => healthChecks.AddBigQuery("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:GCP:BigQuery:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => _ = services.AddSingleton(_ => client),
            cancellationToken: cancellationToken
        );
    }

    [Test]
    public async Task AddBigQuery_UseConfiguration_TimeoutMinusTwo_ThrowException(
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var client = await CreateClientAsync(_database.Endpoint, BigQueryDatabase.ProjectId, cancellationToken);

        await RunAndVerify(
            healthChecks => healthChecks.AddBigQuery("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:GCP:BigQuery:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => _ = services.AddSingleton(_ => client),
            cancellationToken: cancellationToken
        );
    }

    private static async Task<BigQueryClient> CreateClientAsync(
        string endpoint,
        string projectId,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var builder = new BigQueryClientBuilder
        {
            BaseUri = endpoint,
            ProjectId = projectId,
            Credential = GoogleCredential.FromAccessToken("fake-token"),
        };

        return await builder.BuildAsync(cancellationToken).ConfigureAwait(false);
    }
}
