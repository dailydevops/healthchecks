namespace NetEvolve.HealthChecks.Tests.Integration.Apache.ActiveMq;

using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Apache.ActiveMq;
using Testcontainers.ActiveMq;

[TestGroup($"{nameof(Apache)}.{nameof(ActiveMq)}")]
public abstract class ActiveMqHealthCheckBaseTests : HealthCheckTestBase
{
    private readonly IActiveMQAccessor _accessor;

    protected ActiveMqHealthCheckBaseTests(IActiveMQAccessor accessor) => _accessor = accessor;

    [Fact]
    public async Task AddActiveMq_UseOptions_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddActiveMq(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.BrokerAddress = _accessor.BrokerAddress;
                        options.Username = _accessor.Username;
                        options.Password = _accessor.Password;
                        options.Timeout = 500;
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Fact]
    public async Task AddActiveMq_UseOptions_ShouldReturnUnhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddActiveMq(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.BrokerAddress = "invalid";
                        options.Username = _accessor.Username;
                        options.Password = _accessor.Password;
                        options.Timeout = 500;
                    }
                );
            },
            HealthStatus.Unhealthy
        );

    [Fact]
    public async Task AddActiveMq_UseOptions_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddActiveMq(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.BrokerAddress = _accessor.BrokerAddress;
                        options.Username = _accessor.Username;
                        options.Password = _accessor.Password;
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Fact]
    public async Task AddActiveMq_UseConfiguration_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddActiveMq("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:ActiveMq:TestContainerHealthy:BrokerAddress", _accessor.BrokerAddress },
                    { "HealthChecks:ActiveMq:TestContainerHealthy:Username", _accessor.Username },
                    { "HealthChecks:ActiveMq:TestContainerHealthy:Password", _accessor.Password },
                    { "HealthChecks:ActiveMq:TestContainerHealthy:Timeout", "500" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddActiveMq_UseConfiguration_ShouldReturnUnhealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddActiveMq("TestContainerUnhealthy"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:ActiveMq:TestContainerUnhealthy:BrokerAddress", "invalid" },
                    { "HealthChecks:ActiveMq:TestContainerUnhealthy:Username", _accessor.Username },
                    { "HealthChecks:ActiveMq:TestContainerUnhealthy:Password", _accessor.Password },
                    { "HealthChecks:ActiveMq:TestContainerUnhealthy:Timeout", "500" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddActiveMq_UseConfiguration_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddActiveMq("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:ActiveMq:TestContainerDegraded:BrokerAddress", _accessor.BrokerAddress },
                    { "HealthChecks:ActiveMq:TestContainerDegraded:Username", _accessor.Username },
                    { "HealthChecks:ActiveMq:TestContainerDegraded:Password", _accessor.Password },
                    { "HealthChecks:ActiveMq:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddActiveMq_UseConfigration_BrokerAddressStringEmpty_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddActiveMq("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:ActiveMq:TestNoValues:BrokerAddress", "" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddActiveMq_UseConfigration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddActiveMq("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:ActiveMq:TestNoValues:BrokerAddress", _accessor.BrokerAddress },
                    { "HealthChecks:ActiveMq:TestNoValues:Username", _accessor.Username },
                    { "HealthChecks:ActiveMq:TestNoValues:Password", _accessor.Password },
                    { "HealthChecks:ActiveMq:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
