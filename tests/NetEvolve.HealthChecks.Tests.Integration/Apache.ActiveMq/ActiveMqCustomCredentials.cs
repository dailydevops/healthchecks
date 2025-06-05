namespace NetEvolve.HealthChecks.Tests.Integration.Apache.ActiveMq;

using System.Threading.Tasks;
using Testcontainers.ActiveMq;

public sealed class ActiveMqCustomCredentials : IAsyncInitializer, IAsyncDisposable, IActiveMQAccessor
{
    private readonly ArtemisContainer _container;

    public string BrokerAddress => _container.GetBrokerAddress();

    public string? Username { get; } = $"{Guid.NewGuid():D}";

    public string? Password { get; } = $"{Guid.NewGuid():D}";

    public ActiveMqCustomCredentials() =>
        _container = new ArtemisBuilder().WithUsername(Username).WithPassword(Password).Build();

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);
}
