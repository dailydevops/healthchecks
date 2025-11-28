namespace NetEvolve.HealthChecks.Tests.Integration.AWS.DynamoDB;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.AWS.DynamoDB;
using NetEvolve.HealthChecks.Tests.Integration.AWS;

[TestGroup($"{nameof(AWS)}.{nameof(DynamoDB)}")]
[TestGroup("Z01TestGroup")]
[ClassDataSource<LocalStackInstance>(Shared = SharedType.PerClass)]
public class DynamoDbHealthCheckTests : HealthCheckTestBase
{
    private readonly LocalStackInstance _instance;

    public DynamoDbHealthCheckTests(LocalStackInstance instance) => _instance = instance;

    [Test]
    public async Task AddAWSDynamoDB_UseOptionsCreate_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAWSDynamoDB(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.AccessKey = LocalStackInstance.AccessKey;
                        options.SecretKey = LocalStackInstance.SecretKey;
                        options.ServiceUrl = _instance.ConnectionString;
                        options.TableName = LocalStackInstance.TableName;
                        options.Mode = CreationMode.BasicAuthentication;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddAWSDynamoDB_UseOptionsCreate_WhenTableInvalid_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAWSDynamoDB(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.AccessKey = LocalStackInstance.AccessKey;
                        options.SecretKey = LocalStackInstance.SecretKey;
                        options.ServiceUrl = _instance.ConnectionString;
                        options.TableName = "invalid-table";
                        options.Mode = CreationMode.BasicAuthentication;
                    }
                );
            },
            HealthStatus.Unhealthy
        );

    [Test]
    public async Task AddAWSDynamoDB_UseOptionsCreate_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAWSDynamoDB(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.AccessKey = LocalStackInstance.AccessKey;
                        options.SecretKey = LocalStackInstance.SecretKey;
                        options.ServiceUrl = _instance.ConnectionString;
                        options.TableName = LocalStackInstance.TableName;
                        options.Timeout = 0;
                        options.Mode = CreationMode.BasicAuthentication;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddAWSDynamoDB_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAWSDynamoDB("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    ["HealthChecks:AWSDynamoDB:TestContainerHealthy:AccessKey"] = LocalStackInstance.AccessKey,
                    ["HealthChecks:AWSDynamoDB:TestContainerHealthy:TableName"] = LocalStackInstance.TableName,
                    ["HealthChecks:AWSDynamoDB:TestContainerHealthy:SecretKey"] = LocalStackInstance.SecretKey,
                    ["HealthChecks:AWSDynamoDB:TestContainerHealthy:ServiceUrl"] = _instance.ConnectionString,
                    ["HealthChecks:AWSDynamoDB:TestContainerHealthy:Mode"] = "BasicAuthentication",
                    ["HealthChecks:AWSDynamoDB:TestContainerHealthy:Timeout"] = "10000",
                };

                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddAWSDynamoDB_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAWSDynamoDB("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    ["HealthChecks:AWSDynamoDB:TestContainerDegraded:AccessKey"] = LocalStackInstance.AccessKey,
                    ["HealthChecks:AWSDynamoDB:TestContainerDegraded:TableName"] = LocalStackInstance.TableName,
                    ["HealthChecks:AWSDynamoDB:TestContainerDegraded:SecretKey"] = LocalStackInstance.SecretKey,
                    ["HealthChecks:AWSDynamoDB:TestContainerDegraded:ServiceUrl"] = _instance.ConnectionString,
                    ["HealthChecks:AWSDynamoDB:TestContainerDegraded:Mode"] = "BasicAuthentication",
                    ["HealthChecks:AWSDynamoDB:TestContainerDegraded:Timeout"] = "0",
                };

                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddAWSDynamoDB_UseConfiguration_WhenTableInvalid_Unhealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddAWSDynamoDB("TestContainerUnhealthy"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    ["HealthChecks:AWSDynamoDB:TestContainerUnhealthy:AccessKey"] = LocalStackInstance.AccessKey,
                    ["HealthChecks:AWSDynamoDB:TestContainerUnhealthy:TableName"] = "invalid-table",
                    ["HealthChecks:AWSDynamoDB:TestContainerUnhealthy:SecretKey"] = LocalStackInstance.SecretKey,
                    ["HealthChecks:AWSDynamoDB:TestContainerUnhealthy:ServiceUrl"] = _instance.ConnectionString,
                    ["HealthChecks:AWSDynamoDB:TestContainerUnhealthy:Mode"] = "BasicAuthentication",
                };

                _ = config.AddInMemoryCollection(values);
            }
        );
}
