namespace NetEvolve.HealthChecks.Tests.Unit.JanusGraph;

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.JanusGraph;

[TestGroup(nameof(JanusGraph))]
public sealed class JanusGraphConfigureTests
{
    [Test]
    public void Configure_WhenNameIsNull_ThrowArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new JanusGraphConfigure(configuration);
        var options = new JanusGraphOptions();

        // Act
        void Act() => configure.Configure(null, options);

        // Assert
        _ = Assert.Throws<ArgumentException>(Act);
    }

    [Test]
    public void Configure_WhenNameIsEmpty_ThrowArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new JanusGraphConfigure(configuration);
        var options = new JanusGraphOptions();

        // Act
        void Act() => configure.Configure(string.Empty, options);

        // Assert
        _ = Assert.Throws<ArgumentException>(Act);
    }

    [Test]
    public void Configure_WhenNameIsWhitespace_ThrowArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new JanusGraphConfigure(configuration);
        var options = new JanusGraphOptions();

        // Act
        void Act() => configure.Configure(" ", options);

        // Assert
        _ = Assert.Throws<ArgumentException>(Act);
    }

    [Test]
    public async Task Configure_WhenArgumentsAreValid_Expected()
    {
        // Arrange
        var values = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            { "HealthChecks:JanusGraph:Test:KeyedService", "test-key" },
            { "HealthChecks:JanusGraph:Test:Timeout", "1000" },
        };
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(values).Build();
        var configure = new JanusGraphConfigure(configuration);
        var options = new JanusGraphOptions();

        // Act
        configure.Configure("Test", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(options.KeyedService).IsEqualTo("test-key");
            _ = await Assert.That(options.Timeout).IsEqualTo(1000);
        }
    }

    [Test]
    public async Task Validate_WhenNameIsNull_Failed()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new JanusGraphConfigure(configuration);
        var options = new JanusGraphOptions();

        // Act
        var result = configure.Validate(null, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The name cannot be null or whitespace.");
        }
    }

    [Test]
    public async Task Validate_WhenOptionsAreNull_Failed()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new JanusGraphConfigure(configuration);

        // Act
        var result = configure.Validate("Test", null!);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The option cannot be null.");
        }
    }

    [Test]
    public async Task Validate_WhenTimeoutIsInvalid_Failed()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new JanusGraphConfigure(configuration);
        var options = new JanusGraphOptions { Timeout = -2 };

        // Act
        var result = configure.Validate("Test", options);

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
    public async Task Validate_WhenArgumentsAreValid_Success()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new JanusGraphConfigure(configuration);
        var options = new JanusGraphOptions();

        // Act
        var result = configure.Validate("Test", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsTrue();
            _ = await Assert.That(options.CommandAsync).IsEqualTo(JanusGraphHealthCheck.DefaultCommandAsync);
        }
    }
}
