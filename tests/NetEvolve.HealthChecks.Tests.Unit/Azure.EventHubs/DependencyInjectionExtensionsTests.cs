namespace NetEvolve.HealthChecks.Tests.Unit.Azure.EventHubs;

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.EventHubs;

[TestGroup($"{nameof(Azure)}.{nameof(EventHubs)}")]
public class DependencyInjectionExtensionsTests
{
    [Test]
    public void AddAzureEventHubs_WhenArgumentBuilderNull_ThrowArgumentNullException()
    {
        // Arrange
        var builder = default(IHealthChecksBuilder);

        // Act
        void Act() => builder.AddAzureEventHubs("Test");

        // Assert
        _ = Assert.Throws<ArgumentNullException>("builder", Act);
    }

    [Test]
    public void AddAzureEventHubs_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        const string? name = default;

        // Act
        void Act() => builder.AddAzureEventHubs(name!);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("name", Act);
    }

    [Test]
    public void AddAzureEventHubs_WhenArgumentNameEmpty_ThrowArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        var name = string.Empty;

        // Act
        void Act() => builder.AddAzureEventHubs(name);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public void AddAzureEventHubs_WhenArgumentTagsNull_ThrowArgumentNullException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        var tags = default(string[]);

        // Act
        void Act() => builder.AddAzureEventHubs("Test", tags: tags);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("tags", Act);
    }

    [Test]
    public void AddAzureEventHubs_WhenArgumentNameIsAlreadyUsed_ThrowArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        const string name = "Test";

        _ = builder.AddAzureEventHubs(name);

        // Act
        void Act() => builder.AddAzureEventHubs(name);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }
}
