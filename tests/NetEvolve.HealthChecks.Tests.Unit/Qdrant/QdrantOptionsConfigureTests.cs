namespace NetEvolve.HealthChecks.Tests.Unit.Qdrant;

using System;
using System.Threading;
using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Qdrant;

[TestGroup(nameof(Qdrant))]
public sealed class QdrantOptionsConfigureTests
{
    [Test]
    public async Task Validate_WhenArgumentNameNull_ReturnsFail()
    {
        // Arrange
        var options = new QdrantOptions();
        var configure = new QdrantOptionsConfigure(new ConfigurationBuilder().Build());
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
    public async Task Validate_WhenArgumentOptionsNull_ReturnsFail()
    {
        // Arrange
        var configure = new QdrantOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = default(QdrantOptions);

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The option cannot be null.");
        }
    }

    [Test]
    public async Task Validate_WhenArgumentTimeoutLessThanInfinite_ReturnsFail()
    {
        // Arrange
        var configure = new QdrantOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new QdrantOptions { Timeout = -2 };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The timeout cannot be less than infinite (-1).");
        }
    }

    [Test]
    public async Task Validate_WhenArgumentTimeoutValid_ReturnsSuccess()
    {
        // Arrange
        var configure = new QdrantOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new QdrantOptions { Timeout = 100 };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        _ = await Assert.That(result.Succeeded).IsTrue();
    }

    [Test]
    public async Task Validate_WhenArgumentTimeoutInfinite_ReturnsSuccess()
    {
        // Arrange
        var configure = new QdrantOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new QdrantOptions { Timeout = Timeout.Infinite };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        _ = await Assert.That(result.Succeeded).IsTrue();
    }

    [Test]
    public void Configure_WhenArgumentNameNull_ThrowsArgumentNullException()
    {
        // Arrange
        var configure = new QdrantOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = default;
        var options = new QdrantOptions();

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("name", Act);
    }

    [Test]
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

    [Test]
    public void Configure_WhenArgumentNameWhitespace_ThrowsArgumentException()
    {
        // Arrange
        var configure = new QdrantOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = " ";
        var options = new QdrantOptions();

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public async Task Configure_WhenArgumentNameValid_BindsFromConfiguration()
    {
        // Arrange
        var configValues = new Dictionary<string, string?> { ["HealthChecks:Qdrant:Test:Timeout"] = "200" };
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(configValues).Build();
        var configure = new QdrantOptionsConfigure(configuration);
        const string? name = "Test";
        var options = new QdrantOptions();

        // Act
        configure.Configure(name, options);

        // Assert
        _ = await Assert.That(options.Timeout).IsEqualTo(200);
    }
}
