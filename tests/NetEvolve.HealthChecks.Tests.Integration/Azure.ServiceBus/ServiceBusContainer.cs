namespace NetEvolve.HealthChecks.Tests.Integration.Azure.ServiceBus;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.ServiceBus;
using TestContainer = Testcontainers.ServiceBus.ServiceBusContainer;

public sealed class ServiceBusContainer : IAsyncInitializer, IAsyncDisposable
{
    public const string QueueName = "queue.1";
    public const string SubscriptionName = "subscription.1";
    public const string TopicName = "topic.1";

    private readonly TestContainer _container = new ServiceBusBuilder(
        /*dockerimage*/"mcr.microsoft.com/azure-messaging/servicebus-emulator:1.1.2"
    )
        .WithLogger(NullLogger.Instance)
        .WithAcceptLicenseAgreement(true)
        .Build();

    public string ConnectionString => _container.GetConnectionString();

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);
}
