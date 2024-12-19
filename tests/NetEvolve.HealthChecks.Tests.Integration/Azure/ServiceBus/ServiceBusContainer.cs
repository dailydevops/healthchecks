﻿namespace NetEvolve.HealthChecks.Tests.Integration.Azure.ServiceBus;

using System;
using System.Threading.Tasks;
using Testcontainers.ServiceBus;
using TestContainer = Testcontainers.ServiceBus.ServiceBusContainer;

public sealed class ServiceBusContainer : IAsyncLifetime, IAsyncDisposable
{
    private readonly TestContainer _container = new ServiceBusBuilder()
        .WithAcceptLicenseAgreement(true)
        .Build();

    public string ConnectionString => _container.GetConnectionString();

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);

    async Task IAsyncLifetime.DisposeAsync() =>
        await _container.DisposeAsync().ConfigureAwait(false);

    public const string QueueName = "queue.1";
}
