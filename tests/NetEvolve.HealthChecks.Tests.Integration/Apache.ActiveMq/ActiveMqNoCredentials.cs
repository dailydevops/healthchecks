namespace NetEvolve.HealthChecks.Tests.Integration.Apache.ActiveMq;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.ActiveMq;

public sealed class ActiveMqNoCredentials : IAsyncInitializer, IAsyncDisposable, IActiveMQAccessor
{
    private readonly ArtemisContainer _container = new ArtemisBuilder(
        /*dockerimage*/"apache/activemq-artemis:2.44.0"
    )
        .WithLogger(NullLogger.Instance)
        .WithEnvironment("ANONYMOUS_LOGIN", bool.TrueString)
        .Build();

    public string BrokerAddress => _container.GetBrokerAddress();

    public string? Username => null;

    public string? Password => string.Empty;

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);
}
