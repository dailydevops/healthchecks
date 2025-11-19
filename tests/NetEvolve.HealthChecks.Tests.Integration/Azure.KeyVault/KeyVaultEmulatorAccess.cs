namespace NetEvolve.HealthChecks.Tests.Integration.Azure.KeyVault;

using System;
using System.Threading.Tasks;
using AzureKeyVaultEmulator.TestContainers;
using global::Azure.Core;
using global::Azure.Identity;
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

        VaultUri = new Uri(_container.GetEndpoint());
        EmulatorCredential = new EmulatedTokenCredential(VaultUri);

        var options = new SecretClientOptions { DisableChallengeResourceVerification = true };

        var secretClient = new SecretClient(VaultUri, EmulatorCredential, options);
        _ = await secretClient.SetSecretAsync("test-secret", "test-value").ConfigureAwait(false);
    }

    private class EmulatedTokenCredential(Uri vaultUri) : TokenCredential
    {
        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken) =>
            GetTokenAsync(requestContext, cancellationToken).AsTask().GetAwaiter().GetResult();

        public override async ValueTask<AccessToken> GetTokenAsync(
            TokenRequestContext requestContext,
            CancellationToken cancellationToken
        )
        {
            using var client = new HttpClient();

            client.BaseAddress = vaultUri;

#pragma warning disable CA2234 // Pass system uri objects instead of strings
            var response = await client.GetAsync("token", cancellationToken).ConfigureAwait(false);
#pragma warning restore CA2234 // Pass system uri objects instead of strings

            var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            return new AccessToken(content, DateTimeOffset.Now.AddYears(1));
        }
    }
}
