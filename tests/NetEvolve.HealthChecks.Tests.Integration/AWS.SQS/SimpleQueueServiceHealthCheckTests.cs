namespace NetEvolve.HealthChecks.Tests.Integration.AWS.SQS;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.AWS.SQS;

[TestGroup($"{nameof(AWS)}.{nameof(SQS)}")]
[TestGroup("Z01TestGroup")]
[ClassDataSource<FlociStackInstance>(Shared = SharedType.PerClass)]
public class SimpleQueueServiceHealthCheckTests : HealthCheckTestBase
{
    private readonly FlociStackInstance _instance;

    public SimpleQueueServiceHealthCheckTests(FlociStackInstance instance) => _instance = instance;

    [Test]
    public async Task AddAWSSQS_UseOptionsCreate_Healthy(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAWSSQS(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.AccessKey = FlociStackInstance.AccessKey;
                        options.SecretKey = FlociStackInstance.SecretKey;
                        options.ServiceUrl = _instance.ConnectionString;
                        options.QueueName = FlociStackInstance.QueueName;
                        options.Mode = CreationMode.BasicAuthentication;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddAWSSQS_UseOptionsCreate_WhenQueueInvalid_Unhealthy(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAWSSQS(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.AccessKey = FlociStackInstance.AccessKey;
                        options.SecretKey = FlociStackInstance.SecretKey;
                        options.ServiceUrl = _instance.ConnectionString;
                        options.QueueName = "invalid";
                        options.Mode = CreationMode.BasicAuthentication;
                    }
                );
            },
            HealthStatus.Unhealthy,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddAWSSQS_UseOptionsCreate_Degraded(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAWSSQS(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.AccessKey = FlociStackInstance.AccessKey;
                        options.SecretKey = FlociStackInstance.SecretKey;
                        options.ServiceUrl = _instance.ConnectionString;
                        options.QueueName = FlociStackInstance.QueueName;
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
    public async Task AddAWSSQS_UseConfiguration_Healthy(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAWSSQS("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:AWSSQS:TestContainerHealthy:AccessKey", FlociStackInstance.AccessKey },
                    { "HealthChecks:AWSSQS:TestContainerHealthy:SecretKey", FlociStackInstance.SecretKey },
                    { "HealthChecks:AWSSQS:TestContainerHealthy:ServiceUrl", _instance.ConnectionString },
                    { "HealthChecks:AWSSQS:TestContainerHealthy:QueueName", FlociStackInstance.QueueName },
                    { "HealthChecks:AWSSQS:TestContainerHealthy:Mode", nameof(CreationMode.BasicAuthentication) },
                    { "HealthChecks:AWSSQS:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddAWSSQS_UseConfiguration_WhenQueueInvalid_Unhealthy(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAWSSQS("TestContainerUnhealthy"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:AWSSQS:TestContainerUnhealthy:AccessKey", FlociStackInstance.AccessKey },
                    { "HealthChecks:AWSSQS:TestContainerUnhealthy:SecretKey", FlociStackInstance.SecretKey },
                    { "HealthChecks:AWSSQS:TestContainerUnhealthy:ServiceUrl", _instance.ConnectionString },
                    { "HealthChecks:AWSSQS:TestContainerUnhealthy:QueueName", "invalid" },
                    { "HealthChecks:AWSSQS:TestContainerUnhealthy:Mode", nameof(CreationMode.BasicAuthentication) },
                };
                _ = config.AddInMemoryCollection(values);
            },
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddAWSSQS_UseConfiguration_Degraded(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAWSSQS("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:AWSSQS:TestContainerDegraded:AccessKey", FlociStackInstance.AccessKey },
                    { "HealthChecks:AWSSQS:TestContainerDegraded:SecretKey", FlociStackInstance.SecretKey },
                    { "HealthChecks:AWSSQS:TestContainerDegraded:ServiceUrl", _instance.ConnectionString },
                    { "HealthChecks:AWSSQS:TestContainerDegraded:QueueName", FlociStackInstance.QueueName },
                    { "HealthChecks:AWSSQS:TestContainerDegraded:Timeout", "0" },
                    { "HealthChecks:AWSSQS:TestContainerDegraded:Mode", nameof(CreationMode.BasicAuthentication) },
                };
                _ = config.AddInMemoryCollection(values);
            },
            cancellationToken: cancellationToken
        );
}
