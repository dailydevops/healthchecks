namespace NetEvolve.HealthChecks.Tests.Unit.Azure.KeyVault;

using System;
using global::Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.KeyVault;

[TestGroup($"{nameof(Azure)}.{nameof(KeyVault)}")]
public sealed class KeyVaultOptionsConfigureTests
{
    [Test]
    public async Task Validate_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var configure = new KeyVaultOptionsConfigure(configuration, serviceProvider);
        var name = default(string);
        var options = new KeyVaultOptions();

        // Act
        var result = configure.Validate(name, options);

        // Assert
        _ = await Assert.That(result.Failed).IsTrue();
    }

    [Test]
    public async Task Validate_WhenArgumentOptionsNull_ThrowArgumentNullException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var configure = new KeyVaultOptionsConfigure(configuration, serviceProvider);
        var name = "Test";
        var options = default(KeyVaultOptions);

        // Act
        var result = configure.Validate(name, options);

        // Assert
        _ = await Assert.That(result.Failed).IsTrue();
    }

    [Test]
    public async Task Validate_WhenArgumentTimeoutLessThanInfinite_ThrowArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var configure = new KeyVaultOptionsConfigure(configuration, serviceProvider);
        var name = "Test";
        var options = new KeyVaultOptions { Timeout = -2 };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        _ = await Assert.That(result.Failed).IsTrue();
    }

    [Test]
    public async Task Validate_WhenModeIsNull_ThrowArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var configure = new KeyVaultOptionsConfigure(configuration, serviceProvider);
        var name = "Test";
        var options = new KeyVaultOptions { Mode = null };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        _ = await Assert.That(result.Failed).IsTrue();
    }

    [Test]
    public async Task Validate_WhenModeServiceProviderAndServiceNotRegistered_ThrowArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var configure = new KeyVaultOptionsConfigure(configuration, serviceProvider);
        var name = "Test";
        var options = new KeyVaultOptions { Mode = KeyVaultClientCreationMode.ServiceProvider };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        _ = await Assert.That(result.Failed).IsTrue();
    }

    [Test]
    public async Task Validate_WhenModeServiceProviderAndServiceRegistered_ReturnSuccess()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var serviceCollection = new ServiceCollection().AddSingleton<SecretClient>();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var configure = new KeyVaultOptionsConfigure(configuration, serviceProvider);
        var name = "Test";
        var options = new KeyVaultOptions { Mode = KeyVaultClientCreationMode.ServiceProvider };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        _ = await Assert.That(result.Succeeded).IsTrue();
    }

    [Test]
    public async Task Validate_WhenModeDefaultCredentialsAndVaultUriNull_ThrowArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var configure = new KeyVaultOptionsConfigure(configuration, serviceProvider);
        var name = "Test";
        var options = new KeyVaultOptions
        {
            Mode = KeyVaultClientCreationMode.DefaultAzureCredentials,
            VaultUri = null,
        };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        _ = await Assert.That(result.Failed).IsTrue();
    }

    [Test]
    public async Task Validate_WhenModeDefaultCredentialsAndVaultUriRelative_ThrowArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var configure = new KeyVaultOptionsConfigure(configuration, serviceProvider);
        var name = "Test";
        var options = new KeyVaultOptions
        {
            Mode = KeyVaultClientCreationMode.DefaultAzureCredentials,
            VaultUri = new Uri("/relative", UriKind.Relative),
        };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        _ = await Assert.That(result.Failed).IsTrue();
    }

    [Test]
    public async Task Validate_WhenModeDefaultCredentialsAndVaultUriAbsolute_ReturnSuccess()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var configure = new KeyVaultOptionsConfigure(configuration, serviceProvider);
        var name = "Test";
        var options = new KeyVaultOptions
        {
            Mode = KeyVaultClientCreationMode.DefaultAzureCredentials,
            VaultUri = new Uri("https://test.vault.azure.net/"),
        };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        _ = await Assert.That(result.Succeeded).IsTrue();
    }

    [Test]
    public void Configure_WhenArgumentNameNull_ThrowArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var configure = new KeyVaultOptionsConfigure(configuration, serviceProvider);
        var name = default(string);
        var options = new KeyVaultOptions();

        // Act & Assert
        _ = Assert.Throws<ArgumentException>(() => configure.Configure(name, options));
    }
}
