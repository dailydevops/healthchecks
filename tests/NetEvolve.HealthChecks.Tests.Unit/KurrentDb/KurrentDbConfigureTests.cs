namespace NetEvolve.HealthChecks.Tests.Unit.KurrentDb;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.KurrentDb;

[TestGroup(nameof(KurrentDb))]
public sealed class KurrentDbConfigureTests
{
    [Test]
    public async Task Configure_WithNameAndOptions_BindsConfigurationCorrectly()
    {
        // Arrange
        var configValues = new Dictionary<string, string?>
        {
            ["HealthChecks:KurrentDb:TestName:KeyedService"] = "test-key",
            ["HealthChecks:KurrentDb:TestName:Timeout"] = "200",
        };

        var configuration = new ConfigurationBuilder().AddInMemoryCollection(configValues).Build();

        var options = new KurrentDbOptions();
        var configure = new KurrentDbConfigure(configuration);

        // Act
        configure.Configure("TestName", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(options.KeyedService).IsEqualTo("test-key");
            _ = await Assert.That(options.Timeout).IsEqualTo(200);
        }
    }

    [Test]
    public void Configure_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new KurrentDbConfigure(new ConfigurationBuilder().Build());
        const string? name = default;
        var options = new KurrentDbOptions();

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("name", Act);
    }

    [Test]
    public void Configure_WhenArgumentOptionsNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new KurrentDbConfigure(new ConfigurationBuilder().Build());
        var options = new KurrentDbOptions();

        // Act
        void Act() => configure.Configure(options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public async Task Validate_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var options = new KurrentDbOptions();
        var configure = new KurrentDbConfigure(new ConfigurationBuilder().Build());
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
    public async Task Validate_WhenArgumentNameWhitespace_ThrowArgumentInvalidException()
    {
        // Arrange
        var options = new KurrentDbOptions();
        var configure = new KurrentDbConfigure(new ConfigurationBuilder().Build());
        const string name = "";

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
    public async Task Validate_WhenArgumentOptionsNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new KurrentDbConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = default(KurrentDbOptions);

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
    public async Task Validate_WhenArgumentTimeoutLessThanInfinite_ThrowArgumentException()
    {
        // Arrange
        var configure = new KurrentDbConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new KurrentDbOptions { Timeout = -2 };

        // Act
        var result = configure.Validate(name, options);

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
    public async Task Validate_WhenArgumentCommandNull_SetDefaultCommand()
    {
        // Arrange
        var configure = new KurrentDbConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new KurrentDbOptions();

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsTrue();
            _ = await Assert.That(options.CommandAsync).IsEqualTo(KurrentDbHealthCheck.DefaultCommandAsync);
        }
    }
}
