namespace NetEvolve.HealthChecks.Tests.Unit.Cassandra;

using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Cassandra;

[TestGroup(nameof(Cassandra))]
public class CassandraConfigureTests
{
    [Test]
    public void Validate_WhenArgumentNameNull_ReturnFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new CassandraConfigure(configuration);
        const string? name = default;

        // Act
        var result = configure.Validate(name, new CassandraOptions());

        // Assert
        Assert.That(result.Failed, Is.True);
        Assert.That(result.FailureMessage, Is.EqualTo("The name cannot be null or whitespace."));
    }

    [Test]
    public void Validate_WhenArgumentOptionsNull_ReturnFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new CassandraConfigure(configuration);
        const string name = "Test";

        // Act
        var result = configure.Validate(name, null!);

        // Assert
        Assert.That(result.Failed, Is.True);
        Assert.That(result.FailureMessage, Is.EqualTo("The option cannot be null."));
    }

    [Test]
    public void Validate_WhenArgumentTimeoutMinusTwo_ReturnFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new CassandraConfigure(configuration);
        const string name = "Test";

        // Act
        var result = configure.Validate(name, new CassandraOptions { Timeout = -2 });

        // Assert
        Assert.That(result.Failed, Is.True);
        Assert.That(
            result.FailureMessage,
            Is.EqualTo(
                "The timeout value must be a positive number in milliseconds or -1 for an infinite timeout."
            )
        );
    }

    [Test]
    public void Validate_WhenArgumentsValid_ReturnSuccess()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new CassandraConfigure(configuration);
        const string name = "Test";

        // Act
        var result = configure.Validate(name, new CassandraOptions());

        // Assert
        Assert.That(result.Succeeded, Is.True);
    }
}
