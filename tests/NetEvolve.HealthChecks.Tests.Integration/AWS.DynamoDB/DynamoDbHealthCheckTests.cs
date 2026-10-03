namespace NetEvolve.HealthChecks.Tests.Integration.AWS.DynamoDB;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.AWS.DynamoDB;
using NetEvolve.HealthChecks.Tests.Integration.AWS;

[TestGroup($"{nameof(AWS)}.{nameof(DynamoDB)}")]
[TestGroup("Z01TestGroup")]
[ClassDataSource<FlociStackInstance>(Shared = SharedType.PerClass)]
public class DynamoDbHealthCheckTests : HealthCheckTestBase
{
    private readonly FlociStackInstance _instance;

    public DynamoDbHealthCheckTests(FlociStackInstance instance) => _instance = instance;

    [Test]
    public async Task AddAWSDynamoDB_UseOptionsCreate_Healthy(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAWSDynamoDB(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.AccessKey = FlociStackInstance.AccessKey;
                        options.SecretKey = FlociStackInstance.SecretKey;
                        options.ServiceUrl = _instance.ConnectionString;
                        options.TableName = FlociStackInstance.TableName;
                        options.Mode = CreationMode.BasicAuthentication;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddAWSDynamoDB_UseOptionsCreate_WhenTableInvalid_Unhealthy(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAWSDynamoDB(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.AccessKey = FlociStackInstance.AccessKey;
                        options.SecretKey = FlociStackInstance.SecretKey;
                        options.ServiceUrl = _instance.ConnectionString;
                        options.TableName = "invalid-table";
                        options.Mode = CreationMode.BasicAuthentication;
                    }
                );
            },
            HealthStatus.Unhealthy,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddAWSDynamoDB_UseOptionsCreate_Degraded(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAWSDynamoDB(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.AccessKey = FlociStackInstance.AccessKey;
                        options.SecretKey = FlociStackInstance.SecretKey;
                        options.ServiceUrl = _instance.ConnectionString;
                        options.TableName = FlociStackInstance.TableName;
                        options.Timeout = 0;
                        options.Mode = CreationMode.BasicAuthentication;
                    }
                );
            },
            HealthStatus.Degraded,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddAWSDynamoDB_UseConfiguration_Healthy(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAWSDynamoDB("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    ["HealthChecks:AWSDynamoDB:TestContainerHealthy:AccessKey"] = FlociStackInstance.AccessKey,
                    ["HealthChecks:AWSDynamoDB:TestContainerHealthy:TableName"] = FlociStackInstance.TableName,
                    ["HealthChecks:AWSDynamoDB:TestContainerHealthy:SecretKey"] = FlociStackInstance.SecretKey,
                    ["HealthChecks:AWSDynamoDB:TestContainerHealthy:ServiceUrl"] = _instance.ConnectionString,
                    ["HealthChecks:AWSDynamoDB:TestContainerHealthy:Mode"] = "BasicAuthentication",
                    ["HealthChecks:AWSDynamoDB:TestContainerHealthy:Timeout"] = "10000",
                };

                _ = config.AddInMemoryCollection(values);
            },
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddAWSDynamoDB_UseConfiguration_Degraded(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAWSDynamoDB("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    ["HealthChecks:AWSDynamoDB:TestContainerDegraded:AccessKey"] = FlociStackInstance.AccessKey,
                    ["HealthChecks:AWSDynamoDB:TestContainerDegraded:TableName"] = FlociStackInstance.TableName,
                    ["HealthChecks:AWSDynamoDB:TestContainerDegraded:SecretKey"] = FlociStackInstance.SecretKey,
                    ["HealthChecks:AWSDynamoDB:TestContainerDegraded:ServiceUrl"] = _instance.ConnectionString,
                    ["HealthChecks:AWSDynamoDB:TestContainerDegraded:Mode"] = "BasicAuthentication",
                    ["HealthChecks:AWSDynamoDB:TestContainerDegraded:Timeout"] = "0",
                };

                _ = config.AddInMemoryCollection(values);
            },
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddAWSDynamoDB_UseConfiguration_WhenTableInvalid_Unhealthy(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAWSDynamoDB("TestContainerUnhealthy"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    ["HealthChecks:AWSDynamoDB:TestContainerUnhealthy:AccessKey"] = FlociStackInstance.AccessKey,
                    ["HealthChecks:AWSDynamoDB:TestContainerUnhealthy:TableName"] = "invalid-table",
                    ["HealthChecks:AWSDynamoDB:TestContainerUnhealthy:SecretKey"] = FlociStackInstance.SecretKey,
                    ["HealthChecks:AWSDynamoDB:TestContainerUnhealthy:ServiceUrl"] = _instance.ConnectionString,
                    ["HealthChecks:AWSDynamoDB:TestContainerUnhealthy:Mode"] = "BasicAuthentication",
                };

                _ = config.AddInMemoryCollection(values);
            },
            cancellationToken: cancellationToken
        );
}
