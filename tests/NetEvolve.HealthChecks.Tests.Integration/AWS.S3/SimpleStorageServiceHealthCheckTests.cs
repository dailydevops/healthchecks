namespace NetEvolve.HealthChecks.Tests.Integration.AWS.S3;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.AWS.S3;
using NetEvolve.HealthChecks.Tests.Integration.AWS;

[TestGroup($"{nameof(AWS)}.{nameof(S3)}")]
[ClassDataSource<LocalStackInstance>(Shared = SharedType.PerTestSession)]
public class SimpleStorageServiceHealthCheckTests : HealthCheckTestBase
{
    private readonly LocalStackInstance _instance;

    public SimpleStorageServiceHealthCheckTests(LocalStackInstance instance) => _instance = instance;

    [Test]
    public async Task AddAWSS3_UseOptionsCreate_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAWSS3(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.AccessKey = LocalStackInstance.AccessKey;
                        options.SecretKey = LocalStackInstance.SecretKey;
                        options.ServiceUrl = _instance.ConnectionString;
                        options.BucketName = LocalStackInstance.BucketName;
                        options.Mode = CreationMode.BasicAuthentication;
                        options.Timeout = 1000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddAWSS3_UseOptionsCreate_WhenBucketInvalid_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAWSS3(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.AccessKey = LocalStackInstance.AccessKey;
                        options.SecretKey = LocalStackInstance.SecretKey;
                        options.ServiceUrl = _instance.ConnectionString;
                        options.BucketName = "invalid-bucket";
                        options.Mode = CreationMode.BasicAuthentication;
                    }
                );
            },
            HealthStatus.Unhealthy
        );

    [Test]
    public async Task AddAWSS3_UseOptionsCreate_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAWSS3(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.AccessKey = LocalStackInstance.AccessKey;
                        options.SecretKey = LocalStackInstance.SecretKey;
                        options.ServiceUrl = _instance.ConnectionString;
                        options.BucketName = LocalStackInstance.BucketName;
                        options.Timeout = 0;
                        options.Mode = CreationMode.BasicAuthentication;
                    }
                );
            },
            HealthStatus.Degraded
        );

    // Configuration-based tests

    [Test]
    public async Task AddAWSS3_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAWSS3("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    ["HealthChecks:AWSS3:TestContainerHealthy:AccessKey"] = LocalStackInstance.AccessKey,
                    ["HealthChecks:AWSS3:TestContainerHealthy:SecretKey"] = LocalStackInstance.SecretKey,
                    ["HealthChecks:AWSS3:TestContainerHealthy:ServiceUrl"] = _instance.ConnectionString,
                    ["HealthChecks:AWSS3:TestContainerHealthy:BucketName"] = LocalStackInstance.BucketName,
                    ["HealthChecks:AWSS3:TestContainerHealthy:Mode"] = "BasicAuthentication",
                    ["HealthChecks:AWSS3:TestContainerHealthy:Timeout"] = "1000",
                };

                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddAWSS3_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAWSS3("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    ["HealthChecks:AWSS3:TestContainerDegraded:AccessKey"] = LocalStackInstance.AccessKey,
                    ["HealthChecks:AWSS3:TestContainerDegraded:SecretKey"] = LocalStackInstance.SecretKey,
                    ["HealthChecks:AWSS3:TestContainerDegraded:ServiceUrl"] = _instance.ConnectionString,
                    ["HealthChecks:AWSS3:TestContainerDegraded:BucketName"] = LocalStackInstance.BucketName,
                    ["HealthChecks:AWSS3:TestContainerDegraded:Mode"] = "BasicAuthentication",
                    ["HealthChecks:AWSS3:TestContainerDegraded:Timeout"] = "0",
                };

                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddAWSS3_UseConfiguration_WhenBucketInvalid_Unhealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAWSS3("TestContainerUnhealthy"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    ["HealthChecks:AWSS3:TestContainerUnhealthy:AccessKey"] = LocalStackInstance.AccessKey,
                    ["HealthChecks:AWSS3:TestContainerUnhealthy:SecretKey"] = LocalStackInstance.SecretKey,
                    ["HealthChecks:AWSS3:TestContainerUnhealthy:ServiceUrl"] = _instance.ConnectionString,
                    ["HealthChecks:AWSS3:TestContainerUnhealthy:BucketName"] = "invalid-bucket",
                    ["HealthChecks:AWSS3:TestContainerUnhealthy:Mode"] = "BasicAuthentication",
                };

                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddAWSS3_UseConfiguration_WithAdditionalTags() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAWSS3("TestContainerWithTags", tags: ["storage", "custom-tag"]),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    ["HealthChecks:AWSS3:TestContainerWithTags:AccessKey"] = LocalStackInstance.AccessKey,
                    ["HealthChecks:AWSS3:TestContainerWithTags:SecretKey"] = LocalStackInstance.SecretKey,
                    ["HealthChecks:AWSS3:TestContainerWithTags:ServiceUrl"] = _instance.ConnectionString,
                    ["HealthChecks:AWSS3:TestContainerWithTags:BucketName"] = LocalStackInstance.BucketName,
                    ["HealthChecks:AWSS3:TestContainerWithTags:Mode"] = "BasicAuthentication",
                    ["HealthChecks:AWSS3:TestContainerWithTags:Timeout"] = "1000",
                };

                _ = config.AddInMemoryCollection(values);
            }
        );
}
