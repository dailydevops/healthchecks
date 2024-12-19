namespace NetEvolve.HealthChecks.Tests.Integration.Apache.ActiveMq;

using System.Threading.Tasks;
using NetEvolve.HealthChecks.Apache.ActiveMq;
using Testcontainers.ActiveMq;
using TestContainer = Testcontainers.ActiveMq.ArtemisContainer;

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
    public async Task AddActiveMq_UseOptionsCreate_ShouldReturnHealthy() =>
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

    public sealed class CustomCredentials : ActiveMqHealthCheckTests
    {
        private static readonly string Username = $"{Guid.NewGuid():D}";
        private static readonly string Password = $"{Guid.NewGuid():D}";

        public CustomCredentials()
            : base(
                new ArtemisBuilder().WithUsername(Username).WithPassword(Password).Build(),
                Username,
                Password
            ) { }
    }

    public sealed class NoCredentials : ActiveMqHealthCheckTests
    {
        public NoCredentials()
            : base(
                new ArtemisBuilder().WithEnvironment("ANONYMOUS_LOGIN", bool.TrueString).Build(),
                null,
                string.Empty
            ) { }
    }
}
