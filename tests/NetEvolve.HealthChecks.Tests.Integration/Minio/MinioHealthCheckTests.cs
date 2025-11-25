namespace NetEvolve.HealthChecks.Tests.Integration.Minio;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using global::Minio;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Minio;

[ClassDataSource<MinioDatabase>(Shared = SharedType.PerClass)]
[TestGroup(nameof(Minio))]
[TestGroup("Z07TestGroup")]
public class MinioHealthCheckTests : HealthCheckTestBase
{
    private readonly MinioDatabase _database;

    public MinioHealthCheckTests(MinioDatabase database) => _database = database;

    [Test]
    public async Task AddMinio_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddMinio("TestContainerHealthy", options => options.Timeout = 10000),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddSingleton(_database.Client)
        );

    [Test]
    public async Task AddMinio_UseOptionsWithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddMinio(
                    "TestContainerKeyedHealthy",
                    options =>
                    {
                        options.KeyedService = "minio-test";
                        options.Timeout = 10000;
                    }
                ),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddKeyedSingleton("minio-test", (_, _) => _database.Client)
        );

    [Test]
    public async Task AddMinio_UseOptions_WhenBucketInvalid_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddMinio(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.BucketName = "invalid-bucket";
                        options.Timeout = 10000;
                    }
                );
            },
            HealthStatus.Unhealthy,
            serviceBuilder: services => services.AddSingleton(_database.Client)
        );

    [Test]
    public async Task AddMinio_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddMinio(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.CommandAsync = async (client, bucketName, cancellationToken) =>
                        {
                            await Task.Delay(1000, cancellationToken);

                            var bucketExistsArgs = new global::Minio.DataModel.Args.BucketExistsArgs().WithBucket(
                                bucketName
                            );
                            return await client
                                .BucketExistsAsync(bucketExistsArgs, cancellationToken)
                                .ConfigureAwait(false);
                        };
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded,
            serviceBuilder: services => services.AddSingleton(_database.Client)
        );

    [Test]
    public async Task AddMinio_UseOptionsDoubleRegistered_ThrowsArgumentException() =>
        await Assert.ThrowsAsync<ArgumentException>(
            "name",
            async () =>
                await RunAndVerify(
                    healthChecks =>
                        healthChecks.AddMinio("TestContainerHealthy").AddMinio("TestContainerHealthy"),
                    HealthStatus.Healthy,
                    serviceBuilder: services => services.AddSingleton(_database.Client)
                )
        );

    [Test]
    public async Task AddMinio_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddMinio("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Minio:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_database.Client)
        );

    [Test]
    public async Task AddMinio_UseConfigurationWithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddMinio("TestContainerKeyedHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:Minio:TestContainerKeyedHealthy:KeyedService", "minio-test-config" },
                    { "HealthChecks:Minio:TestContainerKeyedHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddKeyedSingleton("minio-test-config", (_, _) => _database.Client)
        );

    [Test]
    public async Task AddMinio_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddMinio("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Minio:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_database.Client)
        );

    [Test]
    public async Task AddMinio_UseConfiguration_TimeoutMinusTwo_ShouldThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddMinio("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Minio:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_database.Client)
        );

    [Test]
    public async Task AddMinio_UseOptions_CommandReturnsFalse_UnhealthyWithMessage() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddMinio(
                    "TestContainerInvalidResult",
                    options => options.CommandAsync = (_, _, _) => Task.FromResult(false)
                );
            },
            HealthStatus.Unhealthy,
            serviceBuilder: services => services.AddSingleton(_database.Client)
        );
}
