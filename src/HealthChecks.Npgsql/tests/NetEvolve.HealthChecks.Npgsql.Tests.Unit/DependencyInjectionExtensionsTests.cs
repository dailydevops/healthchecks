namespace NetEvolve.HealthChecks.Npgsql.Tests.Unit;

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.XUnit;
using Xunit;

[ExcludeFromCodeCoverage]
[UnitTest]
public class DependencyInjectionExtensionsTests
{
    [Fact]
    public void AddPostgreSql_WhenArgumentBuilderNull_ThrowArgumentNullException()
    {
        // Arrange
        var builder = default(IHealthChecksBuilder);

        // Act
        void Act() => _ = builder.AddPostgreSql("Test");

        // Assert
        var ex = Assert.Throws<ArgumentNullException>("builder", Act);
        Assert.Equal("Value cannot be null. (Parameter 'builder')", ex.Message);
    }

    [Fact]
    public void AddPostgreSql_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        var name = default(string);

        // Act
        void Act() => _ = builder.AddPostgreSql(name);

        // Assert
        var ex = Assert.Throws<ArgumentNullException>("name", Act);
        Assert.Equal("Value cannot be null. (Parameter 'name')", ex.Message);
    }

    [Fact]
    public void AddPostgreSql_WhenArgumentNameEmpty_ThrowArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        var name = string.Empty;

        // Act
        void Act() => _ = builder.AddPostgreSql(name);

        // Assert
        var ex = Assert.Throws<ArgumentException>("name", Act);
        Assert.Equal("The value cannot be an empty string. (Parameter 'name')", ex.Message);
    }

    [Fact]
    public void AddPostgreSql_WhenArgumentTagsNull_ThrowArgumentNullException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        var tags = default(string[]);

        // Act
        void Act() => _ = builder.AddPostgreSql("Test", tags: tags);

        // Assert
        var ex = Assert.Throws<ArgumentNullException>("tags", Act);
        Assert.Equal("Value cannot be null. (Parameter 'tags')", ex.Message);
    }

    [Fact]
    public void AddPostgreSql_WhenArgumentNameIsAlreadyUsed_ThrowArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        const string name = "Test";

        // Act
        void Act() => _ = builder.AddPostgreSql(name).AddPostgreSql(name);

        // Assert
        var ex = Assert.Throws<ArgumentException>(nameof(name), Act);
        Assert.StartsWith("Name `Test` already in use.", ex.Message, StringComparison.Ordinal);
    }
}
