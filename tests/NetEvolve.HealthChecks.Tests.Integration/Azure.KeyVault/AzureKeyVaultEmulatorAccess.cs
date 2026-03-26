namespace NetEvolve.HealthChecks.Tests.Integration.Azure.KeyVault;

using System;
using System.Threading.Tasks;
using AzureKeyVaultEmulator.TestContainers;
using global::Azure.Security.KeyVault.Secrets;

public sealed class AzureKeyVaultEmulatorAccess : IAsyncInitializer, IAsyncDisposable
{
    private readonly AzureKeyVaultEmulatorContainer _container = new AzureKeyVaultEmulatorContainer(
        forceCleanupCertificates: true
    );

    public Uri VaultUri { get; private set; } = default!;

    public SecretClient GetSecretClient() => _container.GetSecretClient();

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync()
    {
        await _container.StartAsync().ConfigureAwait(false);

        VaultUri = new Uri(_container.GetEndpoint());
    }
}
