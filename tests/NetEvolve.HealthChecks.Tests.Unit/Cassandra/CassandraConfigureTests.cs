namespace NetEvolve.HealthChecks.Tests.Unit.Cassandra;

using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Cassandra;

[TestGroup(nameof(Cassandra))]
public class CassandraConfigureTests
{
    [Test]
    public async Task Validate_WhenArgumentNameNull_ReturnFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new CassandraConfigure(configuration);
        const string? name = default;

        // Act
        var result = configure.Validate(name, new CassandraOptions());

        // Assert
        _ = await Assert.That(result.Failed).IsTrue();
        _ = await Assert.That(result.FailureMessage).IsEqualTo("The name cannot be null or whitespace.");
    }

    [Test]
    public async Task Validate_WhenArgumentOptionsNull_ReturnFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new CassandraConfigure(configuration);
        const string name = "Test";

        // Act
        var result = configure.Validate(name, null!);

        // Assert
        _ = await Assert.That(result.Failed).IsTrue();
        _ = await Assert.That(result.FailureMessage).IsEqualTo("The option cannot be null.");
    }

    [Test]
    public async Task Validate_WhenArgumentTimeoutMinusTwo_ReturnFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new CassandraConfigure(configuration);
        const string name = "Test";

        // Act
        var result = configure.Validate(name, new CassandraOptions { Timeout = -2 });

        // Assert
        _ = await Assert.That(result.Failed).IsTrue();
        _ = await Assert
            .That(result.FailureMessage)
            .IsEqualTo("The timeout value must be a positive number in milliseconds or -1 for an infinite timeout.");
    }

    [Test]
    public async Task Validate_WhenArgumentsValid_ReturnSuccess()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new CassandraConfigure(configuration);
        const string name = "Test";

        // Act
        var result = configure.Validate(name, new CassandraOptions());

        // Assert
        _ = await Assert.That(result.Succeeded).IsTrue();
    }
}
