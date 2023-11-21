namespace NetEvolve.HealthChecks.SqlServer.Legacy.Tests.Unit;

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.SqlServer.Legacy;
using Xunit;

[ExcludeFromCodeCoverage]
[UnitTest]
public class DependencyInjectionExtensionsTests
{
    [Fact]
    public void AddSqlServer_WhenArgumentBuilderNull_ThrowArgumentNullException()
    {
        // Arrange
        var builder = default(IHealthChecksBuilder);

        // Act
        void Act() => _ = builder.AddSqlServerLegacy("Test");

        // Assert
        _ = Assert.Throws<ArgumentNullException>("builder", Act);
    }

    [Fact]
    public void AddSqlServer_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        var name = default(string);

        // Act
        void Act() => _ = builder.AddSqlServerLegacy(name);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("name", Act);
    }

    [Fact]
    public void AddSqlServer_WhenArgumentNameEmpty_ThrowArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        var name = string.Empty;

        // Act
        void Act() => _ = builder.AddSqlServerLegacy(name);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Fact]
    public void AddSqlServer_WhenArgumentTagsNull_ThrowArgumentNullException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        var tags = default(string[]);

        // Act
        void Act() => _ = builder.AddSqlServerLegacy("Test", tags: tags);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("tags", Act);
    }

    [Fact]
    public void AddSqlServer_WhenArgumentNameIsAlreadyUsed_ThrowArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        const string name = "Test";

        // Act
        void Act() => _ = builder.AddSqlServerLegacy(name).AddSqlServerLegacy(name);

        // Assert
        _ = Assert.Throws<ArgumentException>(nameof(name), Act);
    }
}
