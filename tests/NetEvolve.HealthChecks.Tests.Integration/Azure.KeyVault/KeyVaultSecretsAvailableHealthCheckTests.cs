namespace NetEvolve.HealthChecks.Tests.Integration.Azure.KeyVault;

using System.Threading.Tasks;
using global::Azure.Core;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.KeyVault;

[TestGroup($"{nameof(Azure)}.{nameof(KeyVault)}")]
[ClassDataSource<KeyVaultEmulatorAccess>(Shared = InstanceSharedType.Azure)]
[TestGroup("Z02TestGroup")]
public class KeyVaultSecretsAvailableHealthCheckTests : HealthCheckTestBase
{
    private readonly KeyVaultEmulatorAccess _container;

    public KeyVaultSecretsAvailableHealthCheckTests(KeyVaultEmulatorAccess container) => _container = container;

    [Test]
    public async Task AddKeyVaultSecretsAvailability_UseOptions_ModeServiceProvider_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKeyVaultSecretsAvailability(
                    "ServiceProviderHealthy",
                    options =>
                    {
                        options.Mode = KeyVaultClientCreationMode.ServiceProvider;
                        options.Timeout = 10000;
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
                services.AddAzureClients(clients =>
                    _ = clients.AddSecretClient(_container.VaultUri).WithCredential(_container.EmulatorCredential)
                )
        );

    [Test]
    public async Task AddKeyVaultSecretsAvailability_UseOptions_ModeServiceProvider_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKeyVaultSecretsAvailability(
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
                services.AddAzureClients(clients =>
                    _ = clients.AddSecretClient(_container.VaultUri).WithCredential(_container.EmulatorCredential)
                )
        );

    [Test]
    public async Task AddKeyVaultSecretsAvailability_UseOptions_ModeDefaultAzureCredentials_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKeyVaultSecretsAvailability(
                    "DefaultAzureCredentialsHealthy",
                    options =>
                    {
                        options.Mode = KeyVaultClientCreationMode.DefaultAzureCredentials;
                        options.VaultUri = _container.VaultUri;
                        options.Timeout = 10000;
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
            {
                _ = services.AddSingleton<TokenCredential>(_container.EmulatorCredential);
            }
        );

    [Test]
    public async Task AddKeyVaultSecretsAvailability_UseOptions_ModeDefaultAzureCredentials_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKeyVaultSecretsAvailability(
                    "DefaultAzureCredentialsDegraded",
                    options =>
                    {
                        options.Mode = KeyVaultClientCreationMode.DefaultAzureCredentials;
                        options.VaultUri = _container.VaultUri;
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded,
            serviceBuilder: services =>
            {
                _ = services.AddSingleton<TokenCredential>(_container.EmulatorCredential);
            }
        );
}
