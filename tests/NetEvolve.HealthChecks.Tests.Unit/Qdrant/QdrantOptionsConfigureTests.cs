namespace NetEvolve.HealthChecks.Tests.Unit.Qdrant;

using System;
using System.Threading;
using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Qdrant;
using Xunit;

[TestGroup(nameof(Qdrant))]
public sealed class QdrantOptionsConfigureTests
{
    [Fact]
    public void Validate_WhenArgumentNameNull_ReturnsFail()
    {
        // Arrange
        var options = new QdrantOptions();
        var configure = new QdrantOptionsConfigure(new ConfigurationBuilder().Build());
        var name = default(string);

        // Act
        var result = configure.Validate(name, options);

        // Assert
        Assert.True(result.Failed);
        Assert.Equal("The name cannot be null or whitespace.", result.FailureMessage);
    }

    [Fact]
    public void Validate_WhenArgumentOptionsNull_ReturnsFail()
    {
        // Arrange
        var configure = new QdrantOptionsConfigure(new ConfigurationBuilder().Build());
        var name = "Test";
        var options = default(QdrantOptions);

        // Act
        var result = configure.Validate(name, options);

        // Assert
        Assert.True(result.Failed);
        Assert.Equal("The option cannot be null.", result.FailureMessage);
    }

    [Fact]
    public void Validate_WhenArgumentTimeoutLessThanInfinite_ReturnsFail()
    {
        // Arrange
        var configure = new QdrantOptionsConfigure(new ConfigurationBuilder().Build());
        var name = "Test";
        var options = new QdrantOptions { Timeout = -2 };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        Assert.True(result.Failed);
        Assert.Equal("The timeout cannot be less than infinite (-1).", result.FailureMessage);
    }

    [Fact]
    public void Validate_WhenArgumentTimeoutValid_ReturnsSuccess()
    {
        // Arrange
        var configure = new QdrantOptionsConfigure(new ConfigurationBuilder().Build());
        var name = "Test";
        var options = new QdrantOptions { Timeout = 100 };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        Assert.True(result.Succeeded);
    }

    [Fact]
    public void Validate_WhenArgumentTimeoutInfinite_ReturnsSuccess()
    {
        // Arrange
        var configure = new QdrantOptionsConfigure(new ConfigurationBuilder().Build());
        var name = "Test";
        var options = new QdrantOptions { Timeout = Timeout.Infinite };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        Assert.True(result.Succeeded);
    }

    [Fact]
    public void Configure_WhenArgumentNameNull_ThrowsArgumentNullException()
    {
        // Arrange
        var configure = new QdrantOptionsConfigure(new ConfigurationBuilder().Build());
        var name = default(string);
        var options = new QdrantOptions();

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("name", Act);
    }

    [Fact]
    public void Configure_WhenArgumentNameEmpty_ThrowsArgumentException()
    {
        // Arrange
        var configure = new QdrantOptionsConfigure(new ConfigurationBuilder().Build());
        var name = string.Empty;
        var options = new QdrantOptions();

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Fact]
    public void Configure_WhenArgumentNameWhitespace_ThrowsArgumentException()
    {
        // Arrange
        var configure = new QdrantOptionsConfigure(new ConfigurationBuilder().Build());
        var name = " ";
        var options = new QdrantOptions();

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Fact]
    public void Configure_WhenArgumentNameValid_BindsFromConfiguration()
    {
        // Arrange
        var configValues = new Dictionary<string, string?> { ["HealthChecks:Qdrant:Test:Timeout"] = "200" };
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(configValues).Build();
        var configure = new QdrantOptionsConfigure(configuration);
        var name = "Test";
        var options = new QdrantOptions();

        // Act
        configure.Configure(name, options);

        // Assert
        Assert.Equal(200, options.Timeout);
    }
}
