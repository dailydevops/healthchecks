namespace NetEvolve.HealthChecks.Tests.Integration.Keycloak;

using System.Threading.Tasks;
using Testcontainers.Keycloak;

public sealed class KeycloakDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly KeycloakContainer _database;

    public KeycloakDatabase() =>
        _database = new KeycloakBuilder().WithUsername(Username).WithPassword(Password).Build();

    public string Username { get; } = "test";

    public string Password { get; } = "test";

    public string BaseAddress => _database.GetBaseAddress();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
