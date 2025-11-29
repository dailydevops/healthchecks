namespace NetEvolve.HealthChecks.Tests.Integration.AWS.EC2;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.AWS.EC2;
using NetEvolve.HealthChecks.Tests.Integration.AWS;

[TestGroup($"{nameof(AWS)}.{nameof(EC2)}")]
[TestGroup("Z01TestGroup")]
[ClassDataSource<LocalStackInstance>(Shared = SharedType.PerClass)]
public class ElasticComputeCloudHealthCheckTests : HealthCheckTestBase
{
    private readonly LocalStackInstance _instance;

    public ElasticComputeCloudHealthCheckTests(LocalStackInstance instance) => _instance = instance;

    [Test]
    public async Task AddAWSEC2_UseOptionsCreate_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAWSEC2(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.AccessKey = LocalStackInstance.AccessKey;
                        options.KeyName = "development";
                        options.SecretKey = LocalStackInstance.SecretKey;
                        options.ServiceUrl = _instance.ConnectionString;
                        options.Mode = CreationMode.BasicAuthentication;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddAWSEC2_UseOptionsCreate_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAWSEC2(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.AccessKey = LocalStackInstance.AccessKey;
                        options.KeyName = "development";
                        options.SecretKey = LocalStackInstance.SecretKey;
                        options.ServiceUrl = _instance.ConnectionString;
                        options.Timeout = 0;
                        options.Mode = CreationMode.BasicAuthentication;
                    }
                );
            },
            HealthStatus.Degraded
        );

    // Configuration-based tests

    [Test]
    public async Task AddAWSEC2_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAWSEC2("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    ["HealthChecks:AWSEC2:TestContainerHealthy:AccessKey"] = LocalStackInstance.AccessKey,
                    ["HealthChecks:AWSEC2:TestContainerHealthy:KeyName"] = "development",
                    ["HealthChecks:AWSEC2:TestContainerHealthy:SecretKey"] = LocalStackInstance.SecretKey,
                    ["HealthChecks:AWSEC2:TestContainerHealthy:ServiceUrl"] = _instance.ConnectionString,
                    ["HealthChecks:AWSEC2:TestContainerHealthy:Mode"] = "BasicAuthentication",
                    ["HealthChecks:AWSEC2:TestContainerHealthy:Timeout"] = "10000",
                };

                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddAWSEC2_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAWSEC2("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    ["HealthChecks:AWSEC2:TestContainerDegraded:AccessKey"] = LocalStackInstance.AccessKey,
                    ["HealthChecks:AWSEC2:TestContainerDegraded:KeyName"] = "development",
                    ["HealthChecks:AWSEC2:TestContainerDegraded:SecretKey"] = LocalStackInstance.SecretKey,
                    ["HealthChecks:AWSEC2:TestContainerDegraded:ServiceUrl"] = _instance.ConnectionString,
                    ["HealthChecks:AWSEC2:TestContainerDegraded:Mode"] = "BasicAuthentication",
                    ["HealthChecks:AWSEC2:TestContainerDegraded:Timeout"] = "0",
                };

                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddAWSEC2_UseConfiguration_WithAdditionalTags()
    {
        string[] tags = ["compute", "custom"];
        await RunAndVerify(
            healthChecks => healthChecks.AddAWSEC2("TestContainerWithTags", tags: tags),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    ["HealthChecks:AWSEC2:TestContainerWithTags:AccessKey"] = LocalStackInstance.AccessKey,
                    ["HealthChecks:AWSEC2:TestContainerWithTags:KeyName"] = "development",
                    ["HealthChecks:AWSEC2:TestContainerWithTags:SecretKey"] = LocalStackInstance.SecretKey,
                    ["HealthChecks:AWSEC2:TestContainerWithTags:ServiceUrl"] = _instance.ConnectionString,
                    ["HealthChecks:AWSEC2:TestContainerWithTags:Mode"] = "BasicAuthentication",
                    ["HealthChecks:AWSEC2:TestContainerWithTags:Timeout"] = "10000",
                };

                _ = config.AddInMemoryCollection(values);
            }
        );
    }
}
