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
        var builder = default(IHealthChecksBuilder);

        // Act
        void Act() => builder.AddAzureKeyVault("Test");

        // Assert
        _ = Assert.Throws<ArgumentNullException>("builder", Act);
    }

    [Test]
    public void AddAzureKeyVault_WhenNameIsNull_ThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();
        const string? name = default;

        // Act
        void Act() => builder.AddAzureKeyVault(name!);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("name", Act);
    }

    [Test]
    public void AddAzureKeyVault_WhenNameIsEmpty_ThrowArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();
        var name = string.Empty;

        // Act
        void Act() => builder.AddAzureKeyVault(name);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public void AddAzureKeyVault_WhenTagsIsNull_ThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();
        var name = "Test";
        var tags = default(string[])!;

        // Act
        void Act() => builder.AddAzureKeyVault(name, tags: tags);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("tags", Act);
    }

    [Test]
    public void AddAzureKeyVault_WhenNameIsAlreadyUsed_ThrowArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();
        var name = "Test";

        _ = builder.AddAzureKeyVault(name);

        // Act
        void Act() => builder.AddAzureKeyVault(name);

        // Assert
        _ = Assert.Throws<ArgumentException>(nameof(name), Act);
    }

    [Test]
    public async Task AddAzureKeyVault_WhenNameIsValidAndNotUsed_ReturnBuilder()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();
        var name = "Test";

        // Act
        var result = builder.AddAzureKeyVault(name);

        // Assert
        _ = await Assert.That(result).IsNotNull();
    }

    [Test]
    public async Task AddAzureKeyVault_WhenNameIsValidAndOptionsProvided_ReturnBuilder()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();
        var name = "Test";

        // Act
        var result = builder.AddAzureKeyVault(
            name,
            options =>
            {
                options.VaultUri = new Uri("https://test.vault.azure.net/");
                options.Mode = KeyVaultClientCreationMode.DefaultAzureCredentials;
            }
        );

        // Assert
        _ = await Assert.That(result).IsNotNull();
    }

    [Test]
    public async Task AddAzureKeyVault_WhenNameIsValidAndTagsProvided_ReturnBuilder()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();
        var name = "Test";
        var tags = new[] { "test-tag" };

        // Act
        var result = builder.AddAzureKeyVault(name, tags: tags);

        // Assert
        _ = await Assert.That(result).IsNotNull();
    }
}
