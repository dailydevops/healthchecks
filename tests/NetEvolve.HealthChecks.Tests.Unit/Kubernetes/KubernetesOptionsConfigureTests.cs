namespace NetEvolve.HealthChecks.Tests.Unit.Kubernetes;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Kubernetes;

[TestGroup(nameof(Kubernetes))]
public sealed class KubernetesOptionsConfigureTests
{
    [Test]
    public void Configure_WhenArgumentNameNull_ThrowArgumentException()
    {
        // Arrange
        var configure = new KubernetesOptionsConfigure(new ConfigurationBuilder().Build());
        var options = new KubernetesOptions();

        // Act
        void Act() => configure.Configure(null, options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public async Task Configure_WhenArgumentNameEmpty_ThrowArgumentException()
    {
        // Arrange
        var configure = new KubernetesOptionsConfigure(new ConfigurationBuilder().Build());
        var options = new KubernetesOptions();

        // Act
        void Act() => configure.Configure(string.Empty, options);

        // Assert
        var ex = Assert.Throws<ArgumentException>("name", Act);
        _ = await Assert
            .That(ex.Message)
            .StartsWith("The value cannot be an empty string or composed entirely of whitespace.");
    }

    [Test]
    public async Task Configure_WhenArgumentNameWhiteSpace_ThrowArgumentException()
    {
        // Arrange
        var configure = new KubernetesOptionsConfigure(new ConfigurationBuilder().Build());
        var options = new KubernetesOptions();

        // Act
        void Act() => configure.Configure(" ", options);

        // Assert
        var ex = Assert.Throws<ArgumentException>("name", Act);
        _ = await Assert
            .That(ex.Message)
            .StartsWith("The value cannot be an empty string or composed entirely of whitespace.");
    }

    [Test]
    public async Task Validate_WhenNameNull_ReturnFailure()
    {
        // Arrange
        var configure = new KubernetesOptionsConfigure(new ConfigurationBuilder().Build());
        var options = new KubernetesOptions();

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
    public async Task Validate_WhenOptionsNull_ReturnFailure()
    {
        // Arrange
        var configure = new KubernetesOptionsConfigure(new ConfigurationBuilder().Build());

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
    public async Task Validate_WhenTimeoutLessThanInfinite_ReturnFailure()
    {
        // Arrange
        var configure = new KubernetesOptionsConfigure(new ConfigurationBuilder().Build());
        var options = new KubernetesOptions { Timeout = -2 };

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
    public async Task Validate_WhenOptionsValid_ReturnSuccess()
    {
        // Arrange
        var configure = new KubernetesOptionsConfigure(new ConfigurationBuilder().Build());
        var options = new KubernetesOptions();

        // Act
        var result = configure.Validate("Test", options);

        // Assert
        _ = await Assert.That(result).IsEqualTo(ValidateOptionsResult.Success);
    }

    [Test]
    public async Task Configure_WhenConfigurationAvailable_ExpectedValues()
    {
        // Arrange
        var values = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            { "HealthChecks:Kubernetes:Test:Timeout", "1000" },
            { "HealthChecks:Kubernetes:Test:KeyedService", "test-key" },
        };
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(values).Build();
        var configure = new KubernetesOptionsConfigure(configuration);
        var options = new KubernetesOptions();

        // Act
        configure.Configure("Test", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(options.Timeout).IsEqualTo(1000);
            _ = await Assert.That(options.KeyedService).IsEqualTo("test-key");
        }
    }
}
