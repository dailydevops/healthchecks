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

        // Create the Secret client manually - the emulator provides connection details
        // Based on the documentation, the container should have GetSecretClient() as an instance method
        // Since it's not found, we'll construct the vault URI from the container and use a mock credential
        var port = _container.GetMappedPublicPort(8200);
        VaultUri = new Uri($"https://localhost:{port}");
        
        // Create a custom credential that works with the emulator
        EmulatorCredential = new AzureKeyVaultEmulatorCredential();

        // Create the client manually and prepare a test secret for availability checks
        var secretClient = new SecretClient(VaultUri, EmulatorCredential);
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
