namespace NetEvolve.HealthChecks.Tests.Integration.RabbitMQ;

using System;
using System.Threading.Tasks;
using Testcontainers.RabbitMq;

public sealed class RabbitMQContainer : IAsyncLifetime
{
    private readonly RabbitMqContainer _container = new RabbitMqBuilder().Build();

    public Uri ConnectionString => new Uri(_container.GetConnectionString());

    public async Task DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);
}
