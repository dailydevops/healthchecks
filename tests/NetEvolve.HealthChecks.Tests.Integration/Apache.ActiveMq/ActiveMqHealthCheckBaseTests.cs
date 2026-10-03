namespace NetEvolve.HealthChecks.Tests.Integration.Apache.ActiveMq;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.HealthChecks.Apache.ActiveMq;

public abstract class ActiveMqHealthCheckBaseTests : HealthCheckTestBase
{
    private readonly IActiveMQAccessor _accessor;

    protected ActiveMqHealthCheckBaseTests(IActiveMQAccessor accessor) => _accessor = accessor;

    [Test]
    public async Task AddActiveMq_UseOptions_Healthy(CancellationToken cancellationToken = default) =>
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
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddActiveMq_UseOptions_Unhealthy(CancellationToken cancellationToken = default) =>
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
                        options.Timeout = 10000;
                    }
                );
            },
            HealthStatus.Unhealthy,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddActiveMq_UseOptions_Degraded(CancellationToken cancellationToken = default) =>
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
            HealthStatus.Degraded,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddActiveMq_UseConfiguration_Healthy(CancellationToken cancellationToken = default) =>
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
                    { "HealthChecks:ActiveMq:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddActiveMq_UseConfiguration_Unhealthy(CancellationToken cancellationToken = default) =>
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
                    { "HealthChecks:ActiveMq:TestContainerUnhealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddActiveMq_UseConfiguration_Degraded(CancellationToken cancellationToken = default) =>
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
            },
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddActiveMq_UseConfiguration_BrokerAddressStringEmpty_ThrowException(
        CancellationToken cancellationToken = default
    ) =>
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
            },
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddActiveMq_UseConfiguration_TimeoutMinusTwo_ThrowException(
        CancellationToken cancellationToken = default
    ) =>
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
            },
            cancellationToken: cancellationToken
        );
}
