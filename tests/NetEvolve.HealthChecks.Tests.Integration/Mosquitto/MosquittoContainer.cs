namespace NetEvolve.HealthChecks.Tests.Integration.Mosquitto;

using System;
using System.Threading.Tasks;
using Testcontainers.Mosquitto;

public sealed class MosquittoContainer : IAsyncInitializer, IAsyncDisposable
{
    private readonly Testcontainers.Mosquitto.MosquittoContainer _mosquittoContainer = new MosquittoBuilder().Build();

    public Uri ConnectionString => new Uri(_mosquittoContainer.GetConnectionString(), UriKind.Absolute);

    public async ValueTask DisposeAsync() => await _mosquittoContainer.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _mosquittoContainer.StartAsync().ConfigureAwait(false);
}
