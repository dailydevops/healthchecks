namespace NetEvolve.HealthChecks.Tests.Integration.AWS.CloudWatch;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.AWS.CloudWatch;
using NetEvolve.HealthChecks.Tests.Integration.AWS;

[TestGroup($"{nameof(AWS)}.{nameof(CloudWatch)}")]
[TestGroup("Z01TestGroup")]
[ClassDataSource<FlociStackInstance>(Shared = SharedType.PerClass)]
public class CloudWatchHealthCheckTests : HealthCheckTestBase
{
    private readonly FlociStackInstance _instance;

    public CloudWatchHealthCheckTests(FlociStackInstance instance) => _instance = instance;

    [Test]
    public async Task AddAWSCloudWatch_UseOptionsCreate_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAWSCloudWatch(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.AccessKey = FlociStackInstance.AccessKey;
                        options.SecretKey = FlociStackInstance.SecretKey;
                        options.ServiceUrl = _instance.ConnectionString;
                        options.AlarmName = FlociStackInstance.AlarmName;
                        options.Mode = CreationMode.BasicAuthentication;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddAWSCloudWatch_UseOptionsCreate_WhenAlarmInvalid_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAWSCloudWatch(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.AccessKey = FlociStackInstance.AccessKey;
                        options.SecretKey = FlociStackInstance.SecretKey;
                        options.ServiceUrl = _instance.ConnectionString;
                        options.AlarmName = "Invalid";
                        options.Mode = CreationMode.BasicAuthentication;
                    }
                );
            },
            HealthStatus.Unhealthy
        );

    [Test]
    public async Task AddAWSCloudWatch_UseOptionsCreate_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAWSCloudWatch(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.AccessKey = FlociStackInstance.AccessKey;
                        options.SecretKey = FlociStackInstance.SecretKey;
                        options.ServiceUrl = _instance.ConnectionString;
                        options.AlarmName = FlociStackInstance.AlarmName;
                        options.Timeout = 0;
                        options.Mode = CreationMode.BasicAuthentication;
                    }
                );
            },
            HealthStatus.Degraded
        );

    // Configuration-based tests

    [Test]
    public async Task AddAWSCloudWatch_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAWSCloudWatch("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:AWSCloudWatch:TestContainerHealthy:AccessKey", FlociStackInstance.AccessKey },
                    { "HealthChecks:AWSCloudWatch:TestContainerHealthy:SecretKey", FlociStackInstance.SecretKey },
                    { "HealthChecks:AWSCloudWatch:TestContainerHealthy:ServiceUrl", _instance.ConnectionString },
                    { "HealthChecks:AWSCloudWatch:TestContainerHealthy:AlarmName", FlociStackInstance.AlarmName },
                    {
                        "HealthChecks:AWSCloudWatch:TestContainerHealthy:Mode",
                        nameof(CreationMode.BasicAuthentication)
                    },
                    { "HealthChecks:AWSCloudWatch:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddAWSCloudWatch_UseConfiguration_WhenAlarmInvalid_Unhealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAWSCloudWatch("TestContainerUnhealthy"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:AWSCloudWatch:TestContainerUnhealthy:AccessKey", FlociStackInstance.AccessKey },
                    { "HealthChecks:AWSCloudWatch:TestContainerUnhealthy:SecretKey", FlociStackInstance.SecretKey },
                    { "HealthChecks:AWSCloudWatch:TestContainerUnhealthy:ServiceUrl", _instance.ConnectionString },
                    { "HealthChecks:AWSCloudWatch:TestContainerUnhealthy:AlarmName", "Invalid" },
                    {
                        "HealthChecks:AWSCloudWatch:TestContainerUnhealthy:Mode",
                        nameof(CreationMode.BasicAuthentication)
                    },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddAWSCloudWatch_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAWSCloudWatch("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:AWSCloudWatch:TestContainerDegraded:AccessKey", FlociStackInstance.AccessKey },
                    { "HealthChecks:AWSCloudWatch:TestContainerDegraded:SecretKey", FlociStackInstance.SecretKey },
                    { "HealthChecks:AWSCloudWatch:TestContainerDegraded:ServiceUrl", _instance.ConnectionString },
                    { "HealthChecks:AWSCloudWatch:TestContainerDegraded:AlarmName", FlociStackInstance.AlarmName },
                    { "HealthChecks:AWSCloudWatch:TestContainerDegraded:Timeout", "0" },
                    {
                        "HealthChecks:AWSCloudWatch:TestContainerDegraded:Mode",
                        nameof(CreationMode.BasicAuthentication)
                    },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
