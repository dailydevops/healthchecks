namespace NetEvolve.HealthChecks.Tests.Integration.Minio;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
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
            healthChecks =>
            {
                _ = healthChecks.AddMinio(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.AccessKey = MinioDatabase.AccessKey;
                        options.SecretKey = MinioDatabase.SecretKey;
                        options.ServiceUrl = _database.ConnectionString;
                        options.BucketName = MinioDatabase.BucketName;
                        options.Mode = CreationMode.BasicAuthentication;
                        options.Timeout = 10000;
                    }
                );
            },
            HealthStatus.Healthy
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
                        options.AccessKey = MinioDatabase.AccessKey;
                        options.SecretKey = MinioDatabase.SecretKey;
                        options.ServiceUrl = _database.ConnectionString;
                        options.BucketName = "invalid-bucket";
                        options.Mode = CreationMode.BasicAuthentication;
                    }
                );
            },
            HealthStatus.Unhealthy
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
                        options.AccessKey = MinioDatabase.AccessKey;
                        options.SecretKey = MinioDatabase.SecretKey;
                        options.ServiceUrl = _database.ConnectionString;
                        options.BucketName = MinioDatabase.BucketName;
                        options.Timeout = 0;
                        options.Mode = CreationMode.BasicAuthentication;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddMinio_UseOptionsDoubleRegistered_ThrowsArgumentException() =>
        await Assert.ThrowsAsync<ArgumentException>(
            "name",
            async () =>
                await RunAndVerify(
                    healthChecks =>
                        healthChecks
                            .AddMinio(
                                "TestContainerHealthy",
                                options =>
                                {
                                    options.AccessKey = MinioDatabase.AccessKey;
                                    options.SecretKey = MinioDatabase.SecretKey;
                                    options.ServiceUrl = _database.ConnectionString;
                                    options.BucketName = MinioDatabase.BucketName;
                                    options.Mode = CreationMode.BasicAuthentication;
                                }
                            )
                            .AddMinio("TestContainerHealthy"),
                    HealthStatus.Healthy
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
                    ["HealthChecks:Minio:TestContainerHealthy:AccessKey"] = MinioDatabase.AccessKey,
                    ["HealthChecks:Minio:TestContainerHealthy:SecretKey"] = MinioDatabase.SecretKey,
                    ["HealthChecks:Minio:TestContainerHealthy:ServiceUrl"] = _database.ConnectionString,
                    ["HealthChecks:Minio:TestContainerHealthy:BucketName"] = MinioDatabase.BucketName,
                    ["HealthChecks:Minio:TestContainerHealthy:Mode"] = "BasicAuthentication",
                    ["HealthChecks:Minio:TestContainerHealthy:Timeout"] = "10000",
                };

                _ = config.AddInMemoryCollection(values);
            }
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
                    ["HealthChecks:Minio:TestContainerDegraded:AccessKey"] = MinioDatabase.AccessKey,
                    ["HealthChecks:Minio:TestContainerDegraded:SecretKey"] = MinioDatabase.SecretKey,
                    ["HealthChecks:Minio:TestContainerDegraded:ServiceUrl"] = _database.ConnectionString,
                    ["HealthChecks:Minio:TestContainerDegraded:BucketName"] = MinioDatabase.BucketName,
                    ["HealthChecks:Minio:TestContainerDegraded:Mode"] = "BasicAuthentication",
                    ["HealthChecks:Minio:TestContainerDegraded:Timeout"] = "0",
                };

                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddMinio_UseConfiguration_WhenBucketInvalid_Unhealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddMinio("TestContainerUnhealthy"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    ["HealthChecks:Minio:TestContainerUnhealthy:AccessKey"] = MinioDatabase.AccessKey,
                    ["HealthChecks:Minio:TestContainerUnhealthy:SecretKey"] = MinioDatabase.SecretKey,
                    ["HealthChecks:Minio:TestContainerUnhealthy:ServiceUrl"] = _database.ConnectionString,
                    ["HealthChecks:Minio:TestContainerUnhealthy:BucketName"] = "invalid-bucket",
                    ["HealthChecks:Minio:TestContainerUnhealthy:Mode"] = "BasicAuthentication",
                };

                _ = config.AddInMemoryCollection(values);
            }
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
                    ["HealthChecks:Minio:TestNoValues:AccessKey"] = MinioDatabase.AccessKey,
                    ["HealthChecks:Minio:TestNoValues:SecretKey"] = MinioDatabase.SecretKey,
                    ["HealthChecks:Minio:TestNoValues:ServiceUrl"] = _database.ConnectionString,
                    ["HealthChecks:Minio:TestNoValues:BucketName"] = MinioDatabase.BucketName,
                    ["HealthChecks:Minio:TestNoValues:Mode"] = "BasicAuthentication",
                    ["HealthChecks:Minio:TestNoValues:Timeout"] = "-2",
                };

                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddMinio_UseConfiguration_WithAdditionalTags()
    {
        string[] tags = ["storage", "custom-tag"];
        await RunAndVerify(
            healthChecks => healthChecks.AddMinio("TestContainerWithTags", tags: tags),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    ["HealthChecks:Minio:TestContainerWithTags:AccessKey"] = MinioDatabase.AccessKey,
                    ["HealthChecks:Minio:TestContainerWithTags:SecretKey"] = MinioDatabase.SecretKey,
                    ["HealthChecks:Minio:TestContainerWithTags:ServiceUrl"] = _database.ConnectionString,
                    ["HealthChecks:Minio:TestContainerWithTags:BucketName"] = MinioDatabase.BucketName,
                    ["HealthChecks:Minio:TestContainerWithTags:Mode"] = "BasicAuthentication",
                    ["HealthChecks:Minio:TestContainerWithTags:Timeout"] = "10000",
                };

                _ = config.AddInMemoryCollection(values);
            }
        );
    }
}
