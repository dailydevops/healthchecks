namespace NetEvolve.HealthChecks.Tests.Integration.Azure.KeyVault;

using System;
using System.Threading.Tasks;
using AzureKeyVaultEmulator.TestContainers;
using global::Azure.Identity;
using global::Azure.Security.KeyVault.Secrets;

public sealed class AzureKeyVaultAccess : IAsyncInitializer, IAsyncDisposable
{
    private readonly AzureKeyVaultEmulatorContainer _container = new AzureKeyVaultEmulatorContainerBuilder().Build();

    public Uri VaultUri { get; private set; } = default!;

    public string ConnectionString => VaultUri?.ToString() ?? string.Empty;

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync()
    {
        await _container.StartAsync().ConfigureAwait(false);

        VaultUri = new Uri(_container.GetVaultUri());

        await PrepareKeyVaultRequirements().ConfigureAwait(false);
    }

    private async Task PrepareKeyVaultRequirements()
    {
        var secretClient = new SecretClient(VaultUri, new DefaultAzureCredential());

        // Create a test secret
        _ = await secretClient.SetSecretAsync("test-secret", "test-value").ConfigureAwait(false);
    }
}
