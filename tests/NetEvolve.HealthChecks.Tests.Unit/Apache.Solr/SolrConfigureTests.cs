namespace NetEvolve.HealthChecks.Tests.Unit.Apache.Solr;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Apache.Solr;

[TestGroup($"{nameof(Apache)}.{nameof(Solr)}")]
public sealed class SolrConfigureTests
{
    [Test]
    public async Task Validate_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var options = new SolrOptions();
        var configure = new SolrConfigure(new ConfigurationBuilder().Build());
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
    public async Task Validate_WhenArgumentOptionsNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new SolrConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = default(SolrOptions);

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
    public async Task Validate_WhenBaseUrlNull_ThrowArgumentException()
    {
        // Arrange
        var configure = new SolrConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new SolrOptions { CreationMode = ClientCreationMode.Create };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The BaseUrl cannot be null or whitespace.");
        }
    }

    [Test]
    public async Task Validate_EverythingCorrect_Expected()
    {
        // Arrange
        var configure = new SolrConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new SolrOptions { BaseUrl = "http://localhost:8983" };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        _ = await Assert.That(result.Succeeded).IsTrue();
    }

    [Test]
    public void Configure_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new SolrConfigure(new ConfigurationBuilder().Build());
        const string? name = default;
        var options = new SolrOptions();

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("name", Act);
    }

    [Test]
    public void Configure_WhenArgumentOptionsNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new SolrConfigure(new ConfigurationBuilder().Build());
        var options = new SolrOptions();

        // Act
        void Act() => configure.Configure(options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public async Task Validate_WhenArgumentTimeoutLessThanInfinite_ThrowArgumentException()
    {
        // Arrange
        var configure = new SolrConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new SolrOptions { Timeout = -2 };

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
}
