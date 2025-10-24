namespace NetEvolve.HealthChecks.Tests.Integration.Azure.KeyVault;

using System.Threading.Tasks;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.KeyVault;

[TestGroup($"{nameof(Azure)}.{nameof(KeyVault)}")]
[ClassDataSource<AzureKeyVaultAccess>(Shared = InstanceSharedType.Azure)]
public class KeyVaultSecretAvailableHealthCheckTests : HealthCheckTestBase
{
    private readonly AzureKeyVaultAccess _container;

    public KeyVaultSecretAvailableHealthCheckTests(AzureKeyVaultAccess container) => _container = container;

    [Test]
    public async Task AddKeyVaultSecretAvailability_UseOptions_ModeServiceProvider_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKeyVaultSecretAvailability(
                    "ServiceProviderHealthy",
                    options =>
                    {
                        options.Mode = KeyVaultClientCreationMode.ServiceProvider;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddSecretClient(_container.VaultUri))
        );

    [Test]
    public async Task AddKeyVaultSecretAvailability_UseOptions_ModeServiceProvider_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKeyVaultSecretAvailability(
                    "ServiceProviderDegraded",
                    options =>
                    {
                        options.Mode = KeyVaultClientCreationMode.ServiceProvider;
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded,
            serviceBuilder: services =>
                services.AddAzureClients(clients => _ = clients.AddSecretClient(_container.VaultUri))
        );

    [Test]
    public async Task AddKeyVaultSecretAvailability_UseOptions_ModeDefaultAzureCredentials_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKeyVaultSecretAvailability(
                    "DefaultAzureCredentialsHealthy",
                    options =>
                    {
                        options.Mode = KeyVaultClientCreationMode.DefaultAzureCredentials;
                        options.VaultUri = _container.VaultUri;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddKeyVaultSecretAvailability_UseOptions_ModeDefaultAzureCredentials_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKeyVaultSecretAvailability(
                    "DefaultAzureCredentialsDegraded",
                    options =>
                    {
                        options.Mode = KeyVaultClientCreationMode.DefaultAzureCredentials;
                        options.VaultUri = _container.VaultUri;
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddKeyVaultSecretAvailability_UseOptions_ModeManagedIdentity_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKeyVaultSecretAvailability(
                    "ManagedIdentityHealthy",
                    options =>
                    {
                        options.Mode = KeyVaultClientCreationMode.ManagedIdentity;
                        options.VaultUri = _container.VaultUri;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddKeyVaultSecretAvailability_UseOptions_ModeManagedIdentity_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKeyVaultSecretAvailability(
                    "ManagedIdentityDegraded",
                    options =>
                    {
                        options.Mode = KeyVaultClientCreationMode.ManagedIdentity;
                        options.VaultUri = _container.VaultUri;
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded
        );
}
