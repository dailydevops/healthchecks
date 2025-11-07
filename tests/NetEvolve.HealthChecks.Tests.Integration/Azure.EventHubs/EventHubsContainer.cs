namespace NetEvolve.HealthChecks.Tests.Integration.Azure.EventHubs;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.EventHubs;
using TestContainer = Testcontainers.EventHubs.EventHubsContainer;

public sealed class EventHubsContainer : IAsyncInitializer, IAsyncDisposable
{
    public const string EventHubName = "eventhub.1";

    private readonly TestContainer _container = new EventHubsBuilder()
        .WithLogger(NullLogger.Instance)
        .WithAcceptLicenseAgreement(true)
        .Build();

    public string ConnectionString => _container.GetConnectionString();

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);
}
