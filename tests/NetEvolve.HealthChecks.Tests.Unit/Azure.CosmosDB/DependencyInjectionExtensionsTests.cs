namespace NetEvolve.HealthChecks.Tests.Unit.Azure.CosmosDB;

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.CosmosDB;

[TestGroup($"{nameof(Azure)}.{nameof(CosmosDB)}")]
public class DependencyInjectionExtensionsTests
{
    [Test]
    public void AddCosmosDb_WhenArgumentBuilderNull_ThrowArgumentNullException()
    {
        // Arrange
        var builder = default(IHealthChecksBuilder);

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => builder.AddCosmosDb("Test"));
        Assert.That(exception.ParamName, Is.EqualTo("builder"));
    }

    [Test]
    public void AddCosmosDb_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        const string? name = default;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => builder.AddCosmosDb(name!));
        Assert.That(exception.ParamName, Is.EqualTo("name"));
    }

    [Test]
    public void AddCosmosDb_WhenArgumentNameEmpty_ThrowArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        var name = string.Empty;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => builder.AddCosmosDb(name));
        Assert.That(exception.ParamName, Is.EqualTo("name"));
    }

    [Test]
    public void AddCosmosDb_WhenArgumentTagsNull_ThrowArgumentNullException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        var tags = default(string[]);

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => builder.AddCosmosDb("Test", options => { }, tags!));
        Assert.That(exception.ParamName, Is.EqualTo("tags"));
    }

    [Test]
    public void AddCosmosDb_WhenParametersValid_RegisterHealthCheck()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();

        // Act
        var result = builder.AddCosmosDb("Test");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.SameAs(builder));
    }

    [Test]
    public void AddCosmosDb_WithOptions_RegisterHealthCheck()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();

        // Act
        var result = builder.AddCosmosDb(
            "TestWithOptions",
            options =>
            {
                options.Timeout = 500;
                options.DatabaseName = "TestDb";
            }
        );

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.SameAs(builder));
    }

    [Test]
    public void AddCosmosDb_WithTags_RegisterHealthCheck()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();

        // Act
        var result = builder.AddCosmosDb("TestWithTags", options => { }, "tag1", "tag2");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.SameAs(builder));
    }

    [Test]
    public void AddCosmosDb_MultipleRegistrations_ShouldRegisterAll()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();

        // Act
        var result1 = builder.AddCosmosDb("Test1");
        var result2 = builder.AddCosmosDb("Test2");
        var result3 = builder.AddCosmosDb("Test3");

        // Assert
        Assert.That(result1, Is.SameAs(builder));
        Assert.That(result2, Is.SameAs(builder));
        Assert.That(result3, Is.SameAs(builder));
    }

    [Test]
    public void AddCosmosDb_DuplicateName_ThrowsException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        _ = builder.AddCosmosDb("Duplicate");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => builder.AddCosmosDb("Duplicate"));
        Assert.That(exception.ParamName, Is.EqualTo("name"));
    }
}
