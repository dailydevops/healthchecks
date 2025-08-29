namespace NetEvolve.HealthChecks.Tests.Integration.Azure.KeyVault;

using System;
using System.Threading.Tasks;
using global::Azure.Core;
using global::Azure.Identity;
using global::Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.LowkeyVault;

public sealed class LowkeyVaultAccess : IAsyncInitializer, IAsyncDisposable
{
    private readonly LowkeyVaultContainer _container = new LowkeyVaultBuilder().WithLogger(NullLogger.Instance).Build();

    public Uri VaultUri { get; private set; } = default!;

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync()
    {
        await _container.StartAsync().ConfigureAwait(false);

        // LowkeyVault typically exposes the vault at a specific endpoint
        var baseUri = new Uri(_container.GetConnectionString());
        VaultUri = new Uri(baseUri, "/vault/test-vault");

        // For test containers, we might need to use a mock credential or configure differently
        // LowkeyVault should accept any credentials for testing
    }
}
