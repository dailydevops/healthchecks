namespace NetEvolve.HealthChecks.Tests.Unit.IbmMQ;

using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.IbmMQ;

[TestGroup(nameof(IbmMQ))]
public sealed class IbmMQConfigureTests
{
    [Test]
    public async Task Configure_WithValidConfiguration_ShouldBindOptions()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["HealthChecks:IbmMQ:test:Timeout"] = "500",
                    ["HealthChecks:IbmMQ:test:KeyedService"] = "test-key",
                }
            )
            .Build();

        var configure = new IbmMQConfigure(configuration);
        var options = new IbmMQOptions();

        // Act
        configure.Configure("test", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(options.Timeout).IsEqualTo(500);
            _ = await Assert.That(options.KeyedService).IsEqualTo("test-key");
        }
    }

    [Test]
    public async Task Validate_WithNullName_ShouldFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new IbmMQConfigure(configuration);
        var options = new IbmMQOptions();

        // Act
        var result = configure.Validate(null, options);

        // Assert
        _ = await Assert.That(result.Failed).IsTrue();
    }

    [Test]
    public async Task Validate_WithEmptyName_ShouldFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new IbmMQConfigure(configuration);
        var options = new IbmMQOptions();

        // Act
        var result = configure.Validate(string.Empty, options);

        // Assert
        _ = await Assert.That(result.Failed).IsTrue();
    }

    [Test]
    public async Task Validate_WithNullOptions_ShouldFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new IbmMQConfigure(configuration);

        // Act
        var result = configure.Validate("test", null!);

        // Assert
        _ = await Assert.That(result.Failed).IsTrue();
    }

    [Test]
    public async Task Validate_WithInvalidTimeout_ShouldFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new IbmMQConfigure(configuration);
        var options = new IbmMQOptions { Timeout = -2 };

        // Act
        var result = configure.Validate("test", options);

        // Assert
        _ = await Assert.That(result.Failed).IsTrue();
    }

    [Test]
    public async Task Validate_WithValidOptions_ShouldSucceed()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new IbmMQConfigure(configuration);
        var options = new IbmMQOptions { Timeout = 100 };

        // Act
        var result = configure.Validate("test", options);

        // Assert
        _ = await Assert.That(result.Succeeded).IsTrue();
    }

    [Test]
    public async Task Validate_WithTimeoutInfinite_ShouldSucceed()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new IbmMQConfigure(configuration);
        var options = new IbmMQOptions { Timeout = -1 };

        // Act
        var result = configure.Validate("test", options);

        // Assert
        _ = await Assert.That(result.Succeeded).IsTrue();
    }
}
