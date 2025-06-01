﻿namespace NetEvolve.HealthChecks.Tests.Integration.Apache.ActiveMq;

using System.Threading.Tasks;
using Testcontainers.ActiveMq;

public sealed class ActiveMqDefaultCredentials : IAsyncInitializer, IAsyncDisposable, IActiveMQAccessor
{
    private readonly ArtemisContainer _container = new ArtemisBuilder().Build();

    public string BrokerAddress => _container.GetBrokerAddress();

    public string? Username => ArtemisBuilder.DefaultUsername;

    public string? Password => ArtemisBuilder.DefaultPassword;

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);
}
