namespace NetEvolve.HealthChecks.Tests.Unit.Milvus;

using System;
using System.Threading;
using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Milvus;

[TestGroup(nameof(Milvus))]
public sealed class MilvusOptionsConfigureTests
{
    [Test]
    public async Task Validate_WhenArgumentNameNull_ReturnFailure()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new MilvusOptionsConfigure(configuration);
        const string? name = default;
        var options = new MilvusOptions();

        // Act
        var result = configure.Validate(name, options);

        // Assert
        _ = await Assert.That(result.Failed).IsTrue();
    }

    [Test]
    public async Task Validate_WhenArgumentOptionsNull_ReturnFailure()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new MilvusOptionsConfigure(configuration);
        const string name = "Test";
        var options = default(MilvusOptions);

        // Act
        var result = configure.Validate(name, options);

        // Assert
        _ = await Assert.That(result.Failed).IsTrue();
    }

    [Test]
    public async Task Validate_WhenArgumentTimeoutLessThanInfinite_ReturnFailure()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new MilvusOptionsConfigure(configuration);
        const string name = "Test";
        var options = new MilvusOptions { Timeout = -2 };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        _ = await Assert.That(result.Failed).IsTrue();
    }

    [Test]
    public async Task Validate_WhenArgumentTimeoutEqualInfinite_ReturnSuccess()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new MilvusOptionsConfigure(configuration);
        const string name = "Test";
        var options = new MilvusOptions { Timeout = Timeout.Infinite };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        _ = await Assert.That(result.Succeeded).IsTrue();
    }

    [Test]
    public void Configure_WhenArgumentNameNull_ThrowArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new MilvusOptionsConfigure(configuration);
        const string? name = default;
        var options = new MilvusOptions();

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentException>(Act);
    }
}
