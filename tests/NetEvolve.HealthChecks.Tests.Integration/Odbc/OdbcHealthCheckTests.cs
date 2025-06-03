namespace NetEvolve.HealthChecks.Tests.Integration.Odbc;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Odbc;

[TestGroup(nameof(Odbc))]
public abstract class OdbcHealthCheckTestsBase : HealthCheckTestBase
{
    protected abstract string GetConnectionString();

    [Test]
    public async Task AddOdbc_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddOdbc(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.ConnectionString = GetConnectionString();
                        options.Timeout = 1000; // Set a reasonable timeout
                    });
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddOdbc_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddOdbc(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.ConnectionString = GetConnectionString();
                        options.Command = "SELECT 1; WAITFOR DELAY '00:00:00.100';";
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddOdbc_UseOptions_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddOdbc(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.ConnectionString = GetConnectionString();
                        options.Command = "RAISERROR('This is a test.',16,1)";
                    }
                );
            },
            HealthStatus.Unhealthy
        );

    [Test]
    public async Task AddOdbc_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddOdbc("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Odbc:TestContainerHealthy:ConnectionString", GetConnectionString() },
                    { "HealthChecks:Odbc:TestContainerHealthy:Timeout", "1000" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddOdbc_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddOdbc("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Odbc:TestContainerDegraded:ConnectionString", GetConnectionString() },
                    { "HealthChecks:Odbc:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddOdbc_UseConfigrationConnectionStringEmpty_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddOdbc("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Odbc:TestNoValues:ConnectionString", "" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddOdbc_UseConfigration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddOdbc("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Odbc:TestNoValues:ConnectionString", GetConnectionString() },
                    { "HealthChecks:Odbc:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
