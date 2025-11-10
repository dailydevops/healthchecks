namespace NetEvolve.HealthChecks.Tests.Unit.JanusGraph;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.JanusGraph;

[TestGroup(nameof(JanusGraph))]
public sealed class JanusGraphOptionsTests
{
    [Test]
    public async Task KeyedService_WhenSetToNull_Expected()
    {
        // Arrange
        var options = new JanusGraphOptions { KeyedService = null };

        // Act
        var result = options.KeyedService;

        // Assert
        _ = await Assert.That(result).IsNull();
    }

    [Test]
    public async Task KeyedService_WhenSetToValue_Expected()
    {
        // Arrange
        var expected = "test-key";
        var options = new JanusGraphOptions { KeyedService = expected };

        // Act
        var result = options.KeyedService;

        // Assert
        _ = await Assert.That(result).IsEqualTo(expected);
    }

    [Test]
    public async Task Timeout_WhenDefault_Expected()
    {
        // Arrange
        var options = new JanusGraphOptions();

        // Act
        var result = options.Timeout;

        // Assert
        _ = await Assert.That(result).IsEqualTo(100);
    }

    [Test]
    public async Task Timeout_WhenSetToValue_Expected()
    {
        // Arrange
        var expected = 500;
        var options = new JanusGraphOptions { Timeout = expected };

        // Act
        var result = options.Timeout;

        // Assert
        _ = await Assert.That(result).IsEqualTo(expected);
    }

    [Test]
    public async Task CommandAsync_WhenDefault_Expected()
    {
        // Arrange
        var options = new JanusGraphOptions();

        // Act
        var result = options.CommandAsync;

        // Assert
        _ = await Assert.That(result).IsNotNull();
    }
}
