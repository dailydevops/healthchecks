namespace NetEvolve.HealthChecks.Tests.Unit.Azure.KeyVault;

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.KeyVault;

[TestGroup($"{nameof(Azure)}.{nameof(KeyVault)}")]
public sealed class DependencyInjectionExtensionsTests
{
    [Test]
    public void AddAzureKeyVault_WhenBuilderIsNull_ThrowArgumentNullException()
    {
        // Arrange
        IHealthChecksBuilder builder = null!;
        var name = "Test";

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(() => builder.AddAzureKeyVault(name));
    }

    [Test]
    public void AddAzureKeyVault_WhenNameIsNull_ThrowArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();
        var name = default(string)!;

        // Act & Assert
        _ = Assert.Throws<ArgumentException>(() => builder.AddAzureKeyVault(name));
    }

    [Test]
    public void AddAzureKeyVault_WhenNameIsEmpty_ThrowArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();
        var name = string.Empty;

        // Act & Assert
        _ = Assert.Throws<ArgumentException>(() => builder.AddAzureKeyVault(name));
    }

    [Test]
    public void AddAzureKeyVault_WhenTagsIsNull_ThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();
        var name = "Test";
        var tags = default(string[])!;

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(() => builder.AddAzureKeyVault(name, tags: tags));
    }

    [Test]
    public void AddAzureKeyVault_WhenNameIsAlreadyUsed_ThrowArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();
        var name = "Test";

        _ = builder.AddAzureKeyVault(name);

        // Act & Assert
        _ = Assert.Throws<ArgumentException>(() => builder.AddAzureKeyVault(name));
    }

    [Test]
    public void AddAzureKeyVault_WhenNameIsValidAndNotUsed_ReturnBuilder()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();
        var name = "Test";

        // Act
        var result = builder.AddAzureKeyVault(name);

        // Assert
        _ = Assert.That(result).IsNotNull();
    }

    [Test]
    public void AddAzureKeyVault_WhenNameIsValidAndOptionsProvided_ReturnBuilder()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();
        var name = "Test";

        // Act
        var result = builder.AddAzureKeyVault(name, options =>
        {
            options.VaultUri = new Uri("https://test.vault.azure.net/");
            options.Mode = KeyVaultClientCreationMode.DefaultAzureCredentials;
        });

        // Assert
        _ = Assert.That(result).IsNotNull();
    }

    [Test]
    public void AddAzureKeyVault_WhenNameIsValidAndTagsProvided_ReturnBuilder()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();
        var name = "Test";
        var tags = new[] { "test-tag" };

        // Act
        var result = builder.AddAzureKeyVault(name, tags: tags);

        // Assert
        _ = Assert.That(result).IsNotNull();
    }
}