namespace NetEvolve.HealthChecks.Tests.Unit.Azure.KeyVault;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using global::Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.KeyVault;

[TestGroup($"{nameof(Azure)}.{nameof(KeyVault)}")]
public sealed class KeyVaultSecretAvailableConfigureTests
{
    [Test]
    public void Configure_OnlyOptions_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new KeyVaultSecretAvailableConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );
        var options = new KeyVaultSecretAvailableOptions();

        // Act / Assert
        _ = Assert.Throws<ArgumentException>("name", () => configure.Configure(options));
    }

    [Test]
    [MethodDataSource(nameof(GetValidateTestCases))]
    public async Task Validate_Theory_Expected(
        bool expectedResult,
        string? expectedMessage,
        string? name,
        KeyVaultSecretAvailableOptions options
    )
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new KeyVaultSecretAvailableConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsEqualTo(expectedResult);
            _ = await Assert.That(result.FailureMessage).IsEqualTo(expectedMessage);
        }
    }

    public static IEnumerable<Func<(bool, string?, string?, KeyVaultSecretAvailableOptions)>> GetValidateTestCases()
    {
        yield return () => (false, "The name cannot be null or whitespace.", null, null!);
        yield return () => (false, "The name cannot be null or whitespace.", "\t", null!);
        yield return () => (false, "The option cannot be null.", "name", null!);
        yield return () =>
            (
                false,
                "The timeout value must be a positive number in milliseconds or -1 for an infinite timeout.",
                "name",
                new KeyVaultSecretAvailableOptions { Timeout = -2 }
            );
        yield return () =>
            (
                false,
                "The mode `13` is not supported.",
                "name",
                new KeyVaultSecretAvailableOptions { Mode = (KeyVaultClientCreationMode)13 }
            );

        // Mode: ServiceProvider
        yield return () =>
            (
                false,
                $"No service of type `{nameof(SecretClient)}` registered. Please execute `builder.AddAzureClients()`.",
                "name",
                new KeyVaultSecretAvailableOptions { Mode = KeyVaultClientCreationMode.ServiceProvider }
            );

        // Mode: DefaultAzureCredentials
        yield return () =>
            (
                false,
                "The vault URI cannot be null when using `DefaultAzureCredentials` mode.",
                "name",
                new KeyVaultSecretAvailableOptions { Mode = KeyVaultClientCreationMode.DefaultAzureCredentials }
            );
        yield return () =>
            (
                false,
                "The vault URI must be an absolute URI when using `DefaultAzureCredentials` mode.",
                "name",
                new KeyVaultSecretAvailableOptions
                {
                    Mode = KeyVaultClientCreationMode.DefaultAzureCredentials,
                    VaultUri = new Uri("/relative", UriKind.Relative),
                }
            );
        yield return () =>
            (
                true,
                null,
                "name",
                new KeyVaultSecretAvailableOptions
                {
                    Mode = KeyVaultClientCreationMode.DefaultAzureCredentials,
                    VaultUri = new Uri("https://test.vault.azure.net/", UriKind.Absolute),
                }
            );

        // Mode: ManagedIdentity
        yield return () =>
            (
                false,
                "The vault URI cannot be null when using `ManagedIdentity` mode.",
                "name",
                new KeyVaultSecretAvailableOptions { Mode = KeyVaultClientCreationMode.ManagedIdentity }
            );
        yield return () =>
            (
                false,
                "The vault URI must be an absolute URI when using `ManagedIdentity` mode.",
                "name",
                new KeyVaultSecretAvailableOptions
                {
                    Mode = KeyVaultClientCreationMode.ManagedIdentity,
                    VaultUri = new Uri("/relative", UriKind.Relative),
                }
            );
        yield return () =>
            (
                true,
                null,
                "name",
                new KeyVaultSecretAvailableOptions
                {
                    Mode = KeyVaultClientCreationMode.ManagedIdentity,
                    VaultUri = new Uri("https://test.vault.azure.net/", UriKind.Absolute),
                }
            );

        // Mode: ServicePrincipal
        yield return () =>
            (
                false,
                "The vault URI cannot be null when using `ServicePrincipal` mode.",
                "name",
                new KeyVaultSecretAvailableOptions { Mode = KeyVaultClientCreationMode.ServicePrincipal }
            );
        yield return () =>
            (
                false,
                "The vault URI must be an absolute URI when using `ServicePrincipal` mode.",
                "name",
                new KeyVaultSecretAvailableOptions
                {
                    Mode = KeyVaultClientCreationMode.ServicePrincipal,
                    VaultUri = new Uri("/relative", UriKind.Relative),
                }
            );
        yield return () =>
            (
                false,
                "The tenant ID cannot be null or whitespace when using `ServicePrincipal` mode.",
                "name",
                new KeyVaultSecretAvailableOptions
                {
                    Mode = KeyVaultClientCreationMode.ServicePrincipal,
                    VaultUri = new Uri("https://test.vault.azure.net/", UriKind.Absolute),
                }
            );
        yield return () =>
            (
                false,
                "The client ID cannot be null or whitespace when using `ServicePrincipal` mode.",
                "name",
                new KeyVaultSecretAvailableOptions
                {
                    Mode = KeyVaultClientCreationMode.ServicePrincipal,
                    VaultUri = new Uri("https://test.vault.azure.net/", UriKind.Absolute),
                    TenantId = "tenant-id",
                }
            );
        yield return () =>
            (
                false,
                "The client secret cannot be null or whitespace when using `ServicePrincipal` mode.",
                "name",
                new KeyVaultSecretAvailableOptions
                {
                    Mode = KeyVaultClientCreationMode.ServicePrincipal,
                    VaultUri = new Uri("https://test.vault.azure.net/", UriKind.Absolute),
                    TenantId = "tenant-id",
                    ClientId = "client-id",
                }
            );
        yield return () =>
            (
                true,
                null,
                "name",
                new KeyVaultSecretAvailableOptions
                {
                    Mode = KeyVaultClientCreationMode.ServicePrincipal,
                    VaultUri = new Uri("https://test.vault.azure.net/", UriKind.Absolute),
                    TenantId = "tenant-id",
                    ClientId = "client-id",
                    ClientSecret = "client-secret",
                }
            );
    }
}
