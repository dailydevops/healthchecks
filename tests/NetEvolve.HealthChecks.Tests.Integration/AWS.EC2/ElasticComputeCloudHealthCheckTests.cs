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
[ClassDataSource<LocalStackInstance>(Shared = InstanceSharedType.AWS)]
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
                        options.SecretKey = LocalStackInstance.SecretKey;
                        options.ServiceUrl = _instance.ConnectionString;
                        options.Mode = CreationMode.BasicAuthentication;
                        options.Timeout = 1000; // Set a reasonable timeout
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
                    ["HealthChecks:AWSEC2:TestContainerHealthy:SecretKey"] = LocalStackInstance.SecretKey,
                    ["HealthChecks:AWSEC2:TestContainerHealthy:ServiceUrl"] = _instance.ConnectionString,
                    ["HealthChecks:AWSEC2:TestContainerHealthy:Mode"] = "BasicAuthentication",
                    ["HealthChecks:AWSEC2:TestContainerHealthy:Timeout"] = "1000",
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
        await RunAndVerify(
            healthChecks => healthChecks.AddAWSEC2("TestContainerHealthy", tags: ["custom"]),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    ["HealthChecks:AWSEC2:TestContainerHealthy:AccessKey"] = LocalStackInstance.AccessKey,
                    ["HealthChecks:AWSEC2:TestContainerHealthy:SecretKey"] = LocalStackInstance.SecretKey,
                    ["HealthChecks:AWSEC2:TestContainerHealthy:ServiceUrl"] = _instance.ConnectionString,
                    ["HealthChecks:AWSEC2:TestContainerHealthy:Mode"] = "BasicAuthentication",
                    ["HealthChecks:AWSEC2:TestContainerHealthy:Timeout"] = "1000",
                };

                _ = config.AddInMemoryCollection(values);
            }
        );

        using (Assert.Multiple())
        {
            _ = await Assert
                .That(_response.Entries)
                .HasCount()
                .EqualTo(1);

            var (name, healthReportEntry) = _response.Entries.Single();

            _ = await Assert.That(name).IsEqualTo("TestContainerHealthy");
            _ = await Assert.That(healthReportEntry.Status).IsEqualTo(HealthStatus.Healthy);

            _ = await Assert
                .That(healthReportEntry.Tags)
                .ContainsValue("aws");
            _ = await Assert
                .That(healthReportEntry.Tags)
                .ContainsValue("ec2");
            _ = await Assert
                .That(healthReportEntry.Tags)
                .ContainsValue("compute");
            _ = await Assert
                .That(healthReportEntry.Tags)
                .ContainsValue("custom");
        }
    }
}