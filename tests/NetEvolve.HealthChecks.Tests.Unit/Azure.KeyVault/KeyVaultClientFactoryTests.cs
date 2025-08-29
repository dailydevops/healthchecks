namespace NetEvolve.HealthChecks.Tests.Unit.Azure.KeyVault;

using System;
using global::Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.KeyVault;

[TestGroup($"{nameof(Azure)}.{nameof(KeyVault)}")]
public sealed class KeyVaultClientFactoryTests
{
    [Test]
    public void CreateSecretClient_WhenNameIsNull_ThrowArgumentException()
    {
        // Arrange
        var name = default(string)!;
        var options = new KeyVaultOptions();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        // Act & Assert
        _ = Assert.Throws<ArgumentException>(() =>
            KeyVaultClientFactory.CreateSecretClient(name, options, serviceProvider)
        );
    }

    [Test]
    public void CreateSecretClient_WhenNameIsEmpty_ThrowArgumentException()
    {
        // Arrange
        var name = string.Empty;
        var options = new KeyVaultOptions();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        // Act & Assert
        _ = Assert.Throws<ArgumentException>(() =>
            KeyVaultClientFactory.CreateSecretClient(name, options, serviceProvider)
        );
    }

    [Test]
    public void CreateSecretClient_WhenOptionsIsNull_ThrowArgumentNullException()
    {
        // Arrange
        var name = "Test";
        var options = default(KeyVaultOptions)!;
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(() =>
            KeyVaultClientFactory.CreateSecretClient(name, options, serviceProvider)
        );
    }

    [Test]
    public void CreateSecretClient_WhenServiceProviderIsNull_ThrowArgumentNullException()
    {
        // Arrange
        var name = "Test";
        var options = new KeyVaultOptions();
        var serviceProvider = default(IServiceProvider)!;

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(() =>
            KeyVaultClientFactory.CreateSecretClient(name, options, serviceProvider)
        );
    }

    [Test]
    public void CreateSecretClient_WhenModeServiceProviderAndServiceNotRegistered_ThrowInvalidOperationException()
    {
        // Arrange
        var name = "Test";
        var options = new KeyVaultOptions { Mode = KeyVaultClientCreationMode.ServiceProvider };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        // Act & Assert
        _ = Assert.Throws<InvalidOperationException>(() =>
            KeyVaultClientFactory.CreateSecretClient(name, options, serviceProvider)
        );
    }

    [Test]
    public void CreateSecretClient_WhenModeDefaultCredentialsAndVaultUriNull_ThrowInvalidOperationException()
    {
        // Arrange
        var name = "Test";
        var options = new KeyVaultOptions
        {
            Mode = KeyVaultClientCreationMode.DefaultAzureCredentials,
            VaultUri = null,
        };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        // Act & Assert
        _ = Assert.Throws<InvalidOperationException>(() =>
            KeyVaultClientFactory.CreateSecretClient(name, options, serviceProvider)
        );
    }

    [Test]
    public void CreateSecretClient_WhenModeDefaultCredentialsAndVaultUriValid_ReturnSecretClient()
    {
        // Arrange
        var name = "Test";
        var options = new KeyVaultOptions
        {
            Mode = KeyVaultClientCreationMode.DefaultAzureCredentials,
            VaultUri = new Uri("https://test.vault.azure.net/"),
        };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        // Act
        var result = KeyVaultClientFactory.CreateSecretClient(name, options, serviceProvider);

        // Assert
        _ = Assert.That(result).IsNotNull();
        _ = Assert.That(result).IsInstanceOf<SecretClient>();
    }

    [Test]
    public void CreateSecretClient_WhenModeUnsupported_ThrowNotSupportedException()
    {
        // Arrange
        var name = "Test";
        var options = new KeyVaultOptions { Mode = (KeyVaultClientCreationMode)999 };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        // Act & Assert
        _ = Assert.Throws<NotSupportedException>(() =>
            KeyVaultClientFactory.CreateSecretClient(name, options, serviceProvider)
        );
    }
}
