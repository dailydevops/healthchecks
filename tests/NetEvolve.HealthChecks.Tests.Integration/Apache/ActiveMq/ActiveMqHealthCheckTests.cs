namespace NetEvolve.HealthChecks.Tests.Integration.Apache.ActiveMq;

using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Apache.ActiveMq;
using Testcontainers.ActiveMq;
using TestContainer = Testcontainers.ActiveMq.ArtemisContainer;

[TestGroup($"{nameof(Apache)}.{nameof(ActiveMq)}")]
public abstract class ActiveMqHealthCheckTests : HealthCheckTestBase, IAsyncLifetime
{
    private readonly TestContainer _container;
    private readonly string? _username;
    private readonly string? _password;

    private ActiveMqHealthCheckTests(TestContainer container, string? username, string? password)
    {
        _container = container;
        _username = username;
        _password = password;
    }

    public async Task DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);

    [Fact]
    public async Task AddActiveMq_UseOptions_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddActiveMq(
                "TestContainerHealthy",
                options =>
                {
                    options.BrokerAddress = _container.GetBrokerAddress();
                    options.Username = _username;
                    options.Password = _password;
                    options.Timeout = 500;
                }
            );
        });

    [Fact]
    public async Task AddActiveMq_UseOptions_ShouldReturnUnhealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddActiveMq(
                "TestContainerUnhealthy",
                options =>
                {
                    options.BrokerAddress = "invalid";
                    options.Username = _username;
                    options.Password = _password;
                    options.Timeout = 500;
                }
            );
        });

    [Fact]
    public async Task AddActiveMq_UseOptions_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddActiveMq(
                "TestContainerDegraded",
                options =>
                {
                    options.BrokerAddress = _container.GetBrokerAddress();
                    options.Username = _username;
                    options.Password = _password;
                    options.Timeout = 0;
                }
            );
        });

    [Fact]
    public async Task AddActiveMq_UseConfiguration_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks => _ = healthChecks.AddActiveMq("TestContainerHealthy"),
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:ActiveMq:TestContainerHealthy:BrokerAddress", _container.GetBrokerAddress() },
                    { "HealthChecks:ActiveMq:TestContainerHealthy:Username", _username },
                    { "HealthChecks:ActiveMq:TestContainerHealthy:Password", _password },
                    { "HealthChecks:ActiveMq:TestContainerHealthy:Timeout", "500" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddActiveMq_UseConfiguration_ShouldReturnUnhealthy() =>
        await RunAndVerify(
            healthChecks => _ = healthChecks.AddActiveMq("TestContainerUnhealthy"),
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:ActiveMq:TestContainerUnhealthy:BrokerAddress", "invalid" },
                    { "HealthChecks:ActiveMq:TestContainerUnhealthy:Username", _username },
                    { "HealthChecks:ActiveMq:TestContainerUnhealthy:Password", _password },
                    { "HealthChecks:ActiveMq:TestContainerUnhealthy:Timeout", "500" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddActiveMq_UseConfiguration_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks => _ = healthChecks.AddActiveMq("TestContainerDegraded"),
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:ActiveMq:TestContainerDegraded:BrokerAddress", _container.GetBrokerAddress() },
                    { "HealthChecks:ActiveMq:TestContainerDegraded:Username", _username },
                    { "HealthChecks:ActiveMq:TestContainerDegraded:Password", _password },
                    { "HealthChecks:ActiveMq:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddActiveMq_UseConfigration_BrokerAddressStringEmpty_ThrowException() =>
        await RunAndVerify(
            healthChecks => _ = healthChecks.AddActiveMq("TestNoValues"),
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
            healthChecks => _ = healthChecks.AddActiveMq("TestNoValues"),
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:ActiveMq:TestNoValues:BrokerAddress", _container.GetBrokerAddress() },
                    { "HealthChecks:ActiveMq:TestNoValues:Username", _username },
                    { "HealthChecks:ActiveMq:TestNoValues:Password", _password },
                    { "HealthChecks:ActiveMq:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    public sealed class CustomCredentials : ActiveMqHealthCheckTests
    {
        private static readonly string Username = $"{Guid.NewGuid():D}";
        private static readonly string Password = $"{Guid.NewGuid():D}";

        public CustomCredentials()
            : base(new ArtemisBuilder().WithUsername(Username).WithPassword(Password).Build(), Username, Password) { }
    }

    public sealed class NoCredentials : ActiveMqHealthCheckTests
    {
        public NoCredentials()
            : base(new ArtemisBuilder().WithEnvironment("ANONYMOUS_LOGIN", bool.TrueString).Build(), null, string.Empty)
        { }
    }
}
