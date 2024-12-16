namespace NetEvolve.HealthChecks.Tests.Unit.SQLite;

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.HealthChecks.SQLite;
using Xunit;

public class DependencyInjectionExtensionsTests
{
    [Fact]
    public void AddSQLite_WhenArgumentBuilderNull_ThrowArgumentNullException()
    {
        // Arrange
        var builder = default(IHealthChecksBuilder);

        // Act
        void Act() => _ = builder.AddSQLite("Test");

        // Assert
        _ = Assert.Throws<ArgumentNullException>("builder", Act);
    }

    [Fact]
    public void AddSQLite_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        var name = default(string);

        // Act
        void Act() => _ = builder.AddSQLite(name);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("name", Act);
    }

    [Fact]
    public void AddSQLite_WhenArgumentNameEmpty_ThrowArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        var name = string.Empty;

        // Act
        void Act() => _ = builder.AddSQLite(name);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Fact]
    public void AddSQLite_WhenArgumentTagsNull_ThrowArgumentNullException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        var tags = default(string[]);

        // Act
        void Act() => _ = builder.AddSQLite("Test", tags: tags);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("tags", Act);
    }

    [Fact]
    public void AddSQLite_WhenArgumentNameIsAlreadyUsed_ThrowArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        const string name = "Test";

        // Act
        void Act() => _ = builder.AddSQLite(name).AddSQLite(name);

        // Assert
        _ = Assert.Throws<ArgumentException>(nameof(name), Act);
    }
}
