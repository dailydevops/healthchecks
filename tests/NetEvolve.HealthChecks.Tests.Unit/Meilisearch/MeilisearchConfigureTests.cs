namespace NetEvolve.HealthChecks.Tests.Unit.Meilisearch;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Meilisearch;

[TestGroup(nameof(Meilisearch))]
public class MeilisearchConfigureTests
{
    [Test]
    public void Configure_WhenArgumentNameNull_ThrowArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var configure = new MeilisearchConfigure(configuration, serviceProvider);
        var options = new MeilisearchOptions();
        const string? name = default;

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentException>(nameof(name), Act);
    }

    [Test]
    public async Task Validate_WhenNameNull_ReturnFailed()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var configure = new MeilisearchConfigure(configuration, serviceProvider);
        var options = new MeilisearchOptions();
        const string? name = default;

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The name cannot be null or whitespace.");
        }
    }

    [Test]
    public async Task Validate_WhenOptionsNull_ReturnFailed()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var configure = new MeilisearchConfigure(configuration, serviceProvider);
        const MeilisearchOptions? options = default;
        const string name = "Test";

        // Act
        var result = configure.Validate(name, options!);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The options cannot be null.");
        }
    }

    [Test]
    public async Task Validate_WhenTimeoutMinusTwo_ReturnFailed()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var configure = new MeilisearchConfigure(configuration, serviceProvider);
        var options = new MeilisearchOptions { Timeout = -2 };
        const string name = "Test";

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert
                .That(result.FailureMessage)
                .IsEqualTo(
                    "The timeout value must be a positive number in milliseconds or -1 for an infinite timeout."
                );
        }
    }

    [Test]
    public async Task Validate_WhenHostEmpty_ReturnFailed()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var configure = new MeilisearchConfigure(configuration, serviceProvider);
        var options = new MeilisearchOptions { Host = string.Empty, Mode = MeilisearchClientCreationMode.Internal };
        const string name = "Test";

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert
                .That(result.FailureMessage)
                .IsEqualTo("The host cannot be null or whitespace when using the `Internal` client creation mode.");
        }
    }

    [Test]
    public async Task Validate_WhenClientNotRegistered_ReturnFailed()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var configure = new MeilisearchConfigure(configuration, serviceProvider);
        var options = new MeilisearchOptions { Mode = MeilisearchClientCreationMode.ServiceProvider };
        const string name = "Test";

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert
                .That(result.FailureMessage)
                .IsEqualTo(
                    "No service of type `MeilisearchClient` registered. Please execute `services.AddSingleton<MeilisearchClient>()`."
                );
        }
    }

    [Test]
    public async Task Validate_WhenClientRegistered_ReturnSuccess()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var client = new global::Meilisearch.MeilisearchClient("http://localhost:7700");
        var serviceProvider = new ServiceCollection().AddSingleton(client).BuildServiceProvider();
        var configure = new MeilisearchConfigure(configuration, serviceProvider);
        var options = new MeilisearchOptions { Mode = MeilisearchClientCreationMode.ServiceProvider };
        const string name = "Test";

        // Act
        var result = configure.Validate(name, options);

        // Assert
        _ = await Assert.That(result.Succeeded).IsTrue();
    }

    [Test]
    public async Task Configure_WhenSettingsValid_SetHealthOptions()
    {
        // Arrange
        var values = new Dictionary<string, string?>
        {
            { "HealthChecks:Meilisearch:Test:Host", "http://localhost:7700" },
            { "HealthChecks:Meilisearch:Test:ApiKey", "test-key" },
            { "HealthChecks:Meilisearch:Test:Timeout", "1000" },
        };
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(values).Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var configure = new MeilisearchConfigure(configuration, serviceProvider);
        var options = new MeilisearchOptions();

        // Act
        configure.Configure("Test", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(options.Host).IsEqualTo("http://localhost:7700");
            _ = await Assert.That(options.ApiKey).IsEqualTo("test-key");
            _ = await Assert.That(options.Timeout).IsEqualTo(1000);
        }
    }
}
