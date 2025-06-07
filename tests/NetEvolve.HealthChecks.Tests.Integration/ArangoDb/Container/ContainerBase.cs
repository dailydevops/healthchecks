namespace NetEvolve.HealthChecks.Tests.Integration.ArangoDb.Container;

using System;
using System.Threading.Tasks;
using Testcontainers.ArangoDb;

public abstract class ContainerBase : IAsyncInitializer, IAsyncDisposable
{
    private readonly ArangoDbContainer _container;

    protected ContainerBase(string? password = null)
    {
        var builder = new ArangoDbBuilder();

        if (password is null)
        {
            builder = builder.WithEnvironment("ARANGO_NO_AUTH", "1");
            Password = string.Empty;
        }
        else
        {
            builder = builder.WithPassword(password);
            Password = password;
        }

        _container = builder.Build();
    }

    public string Password { get; }

    public string TransportAddress => _container.GetTransportAddress();

    public async ValueTask DisposeAsync()
    {
        await _container.DisposeAsync().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);
}
