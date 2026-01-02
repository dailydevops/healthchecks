namespace NetEvolve.HealthChecks.Tests.Integration.RabbitMQ;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.RabbitMq;

public sealed class RabbitMQContainer : IAsyncInitializer, IAsyncDisposable
{
    private readonly RabbitMqContainer _container = new RabbitMqBuilder(
        /*dockerimage*/"rabbitmq:3.11.28"
    )
        .WithLogger(NullLogger.Instance)
        .Build();

    public Uri ConnectionString => new Uri(_container.GetConnectionString());

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);
}
