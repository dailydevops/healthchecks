namespace NetEvolve.HealthChecks.Tests.Integration.AWS.S3;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.AWS.S3;
using NetEvolve.HealthChecks.Tests.Integration.AWS;

[TestGroup($"{nameof(AWS)}.{nameof(S3)}")]
[TestGroup("Z01TestGroup")]
[ClassDataSource<FlociStackInstance>(Shared = SharedType.PerClass)]
public class SimpleStorageServiceHealthCheckTests : HealthCheckTestBase
{
    private readonly FlociStackInstance _instance;

    public SimpleStorageServiceHealthCheckTests(FlociStackInstance instance) => _instance = instance;

    [Test]
    public async Task AddAWSS3_UseOptionsCreate_Healthy(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAWSS3(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.AccessKey = FlociStackInstance.AccessKey;
                        options.SecretKey = FlociStackInstance.SecretKey;
                        options.ServiceUrl = _instance.ConnectionString;
                        options.BucketName = FlociStackInstance.BucketName;
                        options.Mode = CreationMode.BasicAuthentication;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddAWSS3_UseOptionsCreate_WhenBucketInvalid_Unhealthy(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAWSS3(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.AccessKey = FlociStackInstance.AccessKey;
                        options.SecretKey = FlociStackInstance.SecretKey;
                        options.ServiceUrl = _instance.ConnectionString;
                        options.BucketName = "invalid-bucket";
                        options.Mode = CreationMode.BasicAuthentication;
                    }
                );
            },
            HealthStatus.Unhealthy,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddAWSS3_UseOptionsCreate_Degraded(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAWSS3(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.AccessKey = FlociStackInstance.AccessKey;
                        options.SecretKey = FlociStackInstance.SecretKey;
                        options.ServiceUrl = _instance.ConnectionString;
                        options.BucketName = FlociStackInstance.BucketName;
                        options.Timeout = 0;
                        options.Mode = CreationMode.BasicAuthentication;
                    }
                );
            },
            HealthStatus.Degraded,
            cancellationToken: cancellationToken
        );

    // Configuration-based tests

    [Test]
    public async Task AddAWSS3_UseConfiguration_Healthy(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAWSS3("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    ["HealthChecks:AWSS3:TestContainerHealthy:AccessKey"] = FlociStackInstance.AccessKey,
                    ["HealthChecks:AWSS3:TestContainerHealthy:SecretKey"] = FlociStackInstance.SecretKey,
                    ["HealthChecks:AWSS3:TestContainerHealthy:ServiceUrl"] = _instance.ConnectionString,
                    ["HealthChecks:AWSS3:TestContainerHealthy:BucketName"] = FlociStackInstance.BucketName,
                    ["HealthChecks:AWSS3:TestContainerHealthy:Mode"] = "BasicAuthentication",
                    ["HealthChecks:AWSS3:TestContainerHealthy:Timeout"] = "1000",
                };

                _ = config.AddInMemoryCollection(values);
            },
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddAWSS3_UseConfiguration_Degraded(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAWSS3("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    ["HealthChecks:AWSS3:TestContainerDegraded:AccessKey"] = FlociStackInstance.AccessKey,
                    ["HealthChecks:AWSS3:TestContainerDegraded:SecretKey"] = FlociStackInstance.SecretKey,
                    ["HealthChecks:AWSS3:TestContainerDegraded:ServiceUrl"] = _instance.ConnectionString,
                    ["HealthChecks:AWSS3:TestContainerDegraded:BucketName"] = FlociStackInstance.BucketName,
                    ["HealthChecks:AWSS3:TestContainerDegraded:Mode"] = "BasicAuthentication",
                    ["HealthChecks:AWSS3:TestContainerDegraded:Timeout"] = "0",
                };

                _ = config.AddInMemoryCollection(values);
            },
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddAWSS3_UseConfiguration_WhenBucketInvalid_Unhealthy(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAWSS3("TestContainerUnhealthy"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    ["HealthChecks:AWSS3:TestContainerUnhealthy:AccessKey"] = FlociStackInstance.AccessKey,
                    ["HealthChecks:AWSS3:TestContainerUnhealthy:SecretKey"] = FlociStackInstance.SecretKey,
                    ["HealthChecks:AWSS3:TestContainerUnhealthy:ServiceUrl"] = _instance.ConnectionString,
                    ["HealthChecks:AWSS3:TestContainerUnhealthy:BucketName"] = "invalid-bucket",
                    ["HealthChecks:AWSS3:TestContainerUnhealthy:Mode"] = "BasicAuthentication",
                };

                _ = config.AddInMemoryCollection(values);
            },
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddAWSS3_UseConfiguration_WithAdditionalTags(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string[] tags = ["storage", "custom-tag"];
        await RunAndVerify(
            healthChecks => healthChecks.AddAWSS3("TestContainerWithTags", tags: tags),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    ["HealthChecks:AWSS3:TestContainerWithTags:AccessKey"] = FlociStackInstance.AccessKey,
                    ["HealthChecks:AWSS3:TestContainerWithTags:SecretKey"] = FlociStackInstance.SecretKey,
                    ["HealthChecks:AWSS3:TestContainerWithTags:ServiceUrl"] = _instance.ConnectionString,
                    ["HealthChecks:AWSS3:TestContainerWithTags:BucketName"] = FlociStackInstance.BucketName,
                    ["HealthChecks:AWSS3:TestContainerWithTags:Mode"] = "BasicAuthentication",
                    ["HealthChecks:AWSS3:TestContainerWithTags:Timeout"] = "1000",
                };

                _ = config.AddInMemoryCollection(values);
            },
            cancellationToken: cancellationToken
        );
    }
}
