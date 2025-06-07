namespace NetEvolve.HealthChecks.Tests.Integration.Keycloak.Container;

using System;
using System.Threading.Tasks;
using Testcontainers.Keycloak;

public abstract class ContainerBase : IAsyncInitializer, IAsyncDisposable
{
    private readonly KeycloakContainer _container;

    protected ContainerBase(
        string username = KeycloakBuilder.DefaultUsername,
        string password = KeycloakBuilder.DefaultPassword
    )
    {
        _container = new KeycloakBuilder().WithPassword(password).WithUsername(username).Build();
        Username = username;
        Password = password;
    }

    public string Username { get; }

    public string Password { get; }

    public string BaseAddress => _container.GetBaseAddress();

    public async ValueTask DisposeAsync()
    {
        await _container.DisposeAsync().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);
}
