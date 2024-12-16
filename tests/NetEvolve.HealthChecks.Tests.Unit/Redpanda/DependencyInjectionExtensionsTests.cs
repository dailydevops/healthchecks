namespace NetEvolve.HealthChecks.Tests.Unit.Redpanda;

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Redpanda;
using Xunit;

public class DependencyInjectionExtensionsTests
{
    [Fact]
    public void AddRedpanda_WhenArgumentBuilderNull_ThrowArgumentNullException()
    {
        // Arrange
        var builder = default(IHealthChecksBuilder);

        // Act
        void Act() => _ = builder.AddRedpanda("Test");

        // Assert
        _ = Assert.Throws<ArgumentNullException>("builder", Act);
    }

    [Fact]
    public void AddRedpanda_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        var name = default(string);

        // Act
        void Act() => _ = builder.AddRedpanda(name);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("name", Act);
    }

    [Fact]
    public void AddRedpanda_WhenArgumentNameEmpty_ThrowArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        var name = string.Empty;

        // Act
        void Act() => _ = builder.AddRedpanda(name);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Fact]
    public void AddRedpanda_WhenArgumentTagsNull_ThrowArgumentNullException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        var tags = default(string[]);

        // Act
        void Act() => _ = builder.AddRedpanda("Test", tags: tags);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("tags", Act);
    }

    [Fact]
    public void AddRedpanda_WhenArgumentNameIsAlreadyUsed_ThrowArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        const string name = "Test";

        // Act
        void Act() => _ = builder.AddRedpanda(name).AddRedpanda(name);

        // Assert
        _ = Assert.Throws<ArgumentException>(nameof(name), Act);
    }
}
