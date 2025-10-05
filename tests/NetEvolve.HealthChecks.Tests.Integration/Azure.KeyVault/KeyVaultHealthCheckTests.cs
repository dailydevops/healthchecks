namespace NetEvolve.HealthChecks.Tests.Integration.Azure.KeyVault;

using System.Threading.Tasks;
using global::Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.KeyVault;
using NetEvolve.HealthChecks.Tests.Integration.Internals;

[TestGroup($"{nameof(Azure)}.{nameof(KeyVault)}")]
[ClassDataSource<LowkeyVaultAccess>(Shared = InstanceSharedType.AzureKeyVault)]
public class KeyVaultHealthCheckTests : HealthCheckTestBase
{
    private readonly LowkeyVaultAccess _container;

    public KeyVaultHealthCheckTests(LowkeyVaultAccess container) => _container = container;

    [Test]
    public async Task AddAzureKeyVault_UseOptions_ModeDefaultAzureCredentials_Healthy() =>
        await RunAndVerify(
                healthChecks =>
                {
                    _ = healthChecks.AddAzureKeyVault(
                        "KeyVaultDefaultCredentialsHealthy",
                        options =>
                        {
                            options.VaultUri = _container.VaultUri;
                            options.Mode = KeyVaultClientCreationMode.DefaultAzureCredentials;
                            options.Timeout = 5000; // Set a reasonable timeout for container tests
                        }
                    );
                },
                HealthStatus.Healthy
            )
            .ConfigureAwait(false);

    [Test]
    public async Task AddAzureKeyVault_UseOptions_ModeServiceProvider_Healthy() =>
        await RunAndVerify(
                healthChecks =>
                {
                    _ = healthChecks.AddAzureKeyVault(
                        "KeyVaultServiceProviderHealthy",
                        options =>
                        {
                            options.Mode = KeyVaultClientCreationMode.ServiceProvider;
                            options.Timeout = 5000; // Set a reasonable timeout
                        }
                    );
                },
                HealthStatus.Healthy,
                serviceBuilder: services =>
                {
                    services.AddAzureClients(azureClientFactoryBuilder =>
                        _ = azureClientFactoryBuilder.AddSecretClient(_container.VaultUri)
                    );
                }
            )
            .ConfigureAwait(false);

    [Test]
    public async Task AddAzureKeyVault_UseConfiguration_Healthy() =>
        await RunAndVerify(
                healthChecks =>
                {
                    _ = healthChecks.AddAzureKeyVault("KeyVaultConfigurationHealthy");
                },
                HealthStatus.Healthy,
                config: configBuilder =>
                {
                    _ = configBuilder.AddJsonStream(
                        new System.IO.MemoryStream(
                            System.Text.Encoding.UTF8.GetBytes(
                                $$$"""
                                {
                                  "HealthChecks": {
                                    "AzureKeyVault": {
                                      "KeyVaultConfigurationHealthy": {
                                        "VaultUri": "{{{_container.VaultUri}}}",
                                        "Mode": "DefaultAzureCredentials",
                                        "Timeout": 5000
                                      }
                                    }
                                  }
                                }
                                """
                            )
                        )
                    );
                }
            )
            .ConfigureAwait(false);

    [Test]
    public async Task AddAzureKeyVault_UseOptions_ModeDefaultAzureCredentials_Unhealthy() =>
        await RunAndVerify(
                healthChecks =>
                {
                    _ = healthChecks.AddAzureKeyVault(
                        "KeyVaultDefaultCredentialsUnhealthy",
                        options =>
                        {
                            options.VaultUri = new System.Uri("https://invalid-vault.vault.azure.net/");
                            options.Mode = KeyVaultClientCreationMode.DefaultAzureCredentials;
                            options.Timeout = 100; // Short timeout for unhealthy test
                        }
                    );
                },
                HealthStatus.Unhealthy
            )
            .ConfigureAwait(false);

    [Test]
    public async Task AddAzureKeyVault_UseOptions_ModeDefaultAzureCredentials_Degraded() =>
        await RunAndVerify(
                healthChecks =>
                {
                    _ = healthChecks.AddAzureKeyVault(
                        "KeyVaultDefaultCredentialsDegraded",
                        options =>
                        {
                            options.VaultUri = _container.VaultUri;
                            options.Mode = KeyVaultClientCreationMode.DefaultAzureCredentials;
                            options.Timeout = 1; // Very short timeout to cause timeout/degraded
                        }
                    );
                },
                HealthStatus.Degraded
            )
            .ConfigureAwait(false);
}
