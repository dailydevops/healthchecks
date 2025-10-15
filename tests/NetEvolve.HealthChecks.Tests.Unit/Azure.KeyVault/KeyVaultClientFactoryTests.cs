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
        const string? name = default;
        var options = new KeyVaultOptions();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        // Act
        void Act() => KeyVaultClientFactory.CreateSecretClient(name!, options, serviceProvider);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public void CreateSecretClient_WhenNameIsEmpty_ThrowArgumentException()
    {
        // Arrange
        var name = string.Empty;
        var options = new KeyVaultOptions();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        // Act
        void Act() => KeyVaultClientFactory.CreateSecretClient(name, options, serviceProvider);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public void CreateSecretClient_WhenOptionsIsNull_ThrowArgumentNullException()
    {
        // Arrange
        var name = "Test";
        var options = default(KeyVaultOptions)!;
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        // Act
        void Act() => KeyVaultClientFactory.CreateSecretClient(name, options, serviceProvider);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("options", Act);
    }

    [Test]
    public void CreateSecretClient_WhenServiceProviderIsNull_ThrowArgumentNullException()
    {
        // Arrange
        var name = "Test";
        var options = new KeyVaultOptions();
        var serviceProvider = default(IServiceProvider)!;

        // Act
        void Act() => KeyVaultClientFactory.CreateSecretClient(name, options, serviceProvider);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("serviceProvider", Act);
    }

    [Test]
    public void CreateSecretClient_WhenModeServiceProviderAndServiceNotRegistered_ThrowInvalidOperationException()
    {
        // Arrange
        var name = "Test";
        var options = new KeyVaultOptions { Mode = KeyVaultClientCreationMode.ServiceProvider };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        // Act
        void Act() => KeyVaultClientFactory.CreateSecretClient(name, options, serviceProvider);

        // Assert
        _ = Assert.Throws<InvalidOperationException>(Act);
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

        // Act
        void Act() => KeyVaultClientFactory.CreateSecretClient(name, options, serviceProvider);

        // Assert
        _ = Assert.Throws<InvalidOperationException>(Act);
    }

    [Test]
    public async Task CreateSecretClient_WhenModeDefaultCredentialsAndVaultUriValid_ReturnSecretClient()
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
        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsNotNull();
            _ = await Assert.That(result).IsTypeOf<SecretClient>();
        }
    }

    [Test]
    public void CreateSecretClient_WhenModeUnsupported_ThrowNotSupportedException()
    {
        // Arrange
        var name = "Test";
        var options = new KeyVaultOptions { Mode = (KeyVaultClientCreationMode)999 };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        // Act
        void Act() => KeyVaultClientFactory.CreateSecretClient(name, options, serviceProvider);

        // Assert
        _ = Assert.Throws<NotSupportedException>(Act);
    }
}
