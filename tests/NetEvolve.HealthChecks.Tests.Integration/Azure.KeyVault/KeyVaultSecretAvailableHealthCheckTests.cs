namespace NetEvolve.HealthChecks.Tests.Integration.Azure.KeyVault;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.KeyVault;

[TestGroup("Azure.KeyVault")]
[TestGroup("Z02TestGroup")]
[ClassDataSource<AzureKeyVaultEmulatorAccess>(Shared = SharedType.PerClass)]
[Skip("AzureKeyVaultEmulator requires SSL certificate installation and may require elevated privileges.")]
public class KeyVaultSecretAvailableHealthCheckTests : HealthCheckTestBase
{
    private readonly AzureKeyVaultEmulatorAccess _container;

    public KeyVaultSecretAvailableHealthCheckTests(AzureKeyVaultEmulatorAccess container) =>
        _container = container;

    [Test]
    public async Task AddKeyVaultSecretAvailability_UseOptions_ModeServiceProvider_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKeyVaultSecretAvailability(
                    "KeyVaultServiceProviderHealthy",
                    options =>
                    {
                        options.Mode = KeyVaultClientCreationMode.ServiceProvider;
                        options.Timeout = 10000;
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
                services.AddSingleton(_container.GetSecretClient())
        );

    [Test]
    public async Task AddKeyVaultSecretAvailability_UseOptions_ModeServiceProvider_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKeyVaultSecretAvailability(
                    "KeyVaultServiceProviderDegraded",
                    options =>
                    {
                        options.Mode = KeyVaultClientCreationMode.ServiceProvider;
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded,
            serviceBuilder: services =>
                services.AddSingleton(_container.GetSecretClient())
        );

    [Test]
    public async Task AddKeyVaultSecretAvailability_UseOptions_ModeDefaultAzureCredentials_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKeyVaultSecretAvailability(
                    "KeyVaultDefaultAzureCredentialsHealthy",
                    options =>
                    {
                        options.VaultUri = _container.VaultUri;
                        options.Mode = KeyVaultClientCreationMode.DefaultAzureCredentials;
                        options.Timeout = 10000;
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
                    "KeyVaultDefaultAzureCredentialsDegraded",
                    options =>
                    {
                        options.VaultUri = _container.VaultUri;
                        options.Mode = KeyVaultClientCreationMode.DefaultAzureCredentials;
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    [Skip("ClientSecretCredential mode is not supported by the Azure Key Vault Emulator.")]
    public async Task AddKeyVaultSecretAvailability_UseOptions_ModeClientSecretCredential_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKeyVaultSecretAvailability(
                    "KeyVaultClientSecretCredentialHealthy",
                    options =>
                    {
                        options.VaultUri = _container.VaultUri;
                        options.Mode = KeyVaultClientCreationMode.ClientSecretCredential;
                        options.TenantId = "test-tenant";
                        options.ClientId = "test-client";
                        options.ClientSecret = "test-secret";
                        options.Timeout = 10000;
                    }
                );
            },
            HealthStatus.Healthy
        );

    // Configuration-based tests

    [Test]
    public async Task AddKeyVaultSecretAvailability_UseConfiguration_ModeServiceProvider_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddKeyVaultSecretAvailability("KeyVaultConfigurationHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(System.StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:AzureKeyVaultSecret:KeyVaultConfigurationHealthy:Mode",
                        nameof(KeyVaultClientCreationMode.ServiceProvider)
                    },
                    { "HealthChecks:AzureKeyVaultSecret:KeyVaultConfigurationHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services =>
                services.AddSingleton(_container.GetSecretClient())
        );

    [Test]
    public async Task AddKeyVaultSecretAvailability_UseConfiguration_ModeServiceProvider_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddKeyVaultSecretAvailability("KeyVaultConfigurationDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(System.StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:AzureKeyVaultSecret:KeyVaultConfigurationDegraded:Mode",
                        nameof(KeyVaultClientCreationMode.ServiceProvider)
                    },
                    { "HealthChecks:AzureKeyVaultSecret:KeyVaultConfigurationDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services =>
                services.AddSingleton(_container.GetSecretClient())
        );
}
