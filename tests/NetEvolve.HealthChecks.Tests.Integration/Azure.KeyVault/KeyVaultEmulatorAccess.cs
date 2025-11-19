namespace NetEvolve.HealthChecks.Tests.Integration.Azure.KeyVault;

using System;
using System.Threading.Tasks;
using AzureKeyVaultEmulator.TestContainers;
using global::Azure.Core;
using global::Azure.Security.KeyVault.Secrets;

public sealed class KeyVaultEmulatorAccess : IAsyncInitializer, IAsyncDisposable
{
    private readonly AzureKeyVaultEmulatorContainer _container = new();

    public Uri VaultUri { get; private set; } = default!;

    public TokenCredential EmulatorCredential { get; private set; } = default!;

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync()
    {
        await _container.StartAsync().ConfigureAwait(false);

        // Use the extension methods provided by the package to get configured clients
        var secretClient = _container.GetSecretClient();
        VaultUri = secretClient.VaultUri;
        
        // Create a custom credential that works with the emulator
        EmulatorCredential = new AzureKeyVaultEmulatorCredential();

        // Prepare a test secret for availability checks
        _ = await secretClient.SetSecretAsync("test-secret", "test-value").ConfigureAwait(false);
    }
}

// Custom credential that wraps the container's client configuration
internal sealed class AzureKeyVaultEmulatorCredential : TokenCredential
{
    public override AccessToken GetToken(TokenRequestContext requestContext, System.Threading.CancellationToken cancellationToken) =>
        new("emulator-token", DateTimeOffset.MaxValue);

    public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, System.Threading.CancellationToken cancellationToken) =>
        new(GetToken(requestContext, cancellationToken));
}
