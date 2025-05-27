namespace NetEvolve.HealthChecks.Tests.Unit.RabbitMQ;

using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.RabbitMQ;
using Xunit;

[TestGroup(nameof(RabbitMQ))]
public sealed class RabbitMQOptionsTests
{
    [Fact]
    public void Constructor_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var options = new RabbitMQOptions();

        // Assert
        Assert.Null(options.KeyedService);
        Assert.Equal(100, options.Timeout);
    }

    [Fact]
    public void KeyedService_SetAndGet_WorksCorrectly()
    {
        // Arrange
        var options = new RabbitMQOptions();
        const string testValue = "test-keyed-service";

        // Act
        options.KeyedService = testValue;

        // Assert
        Assert.Equal(testValue, options.KeyedService);
    }

    [Fact]
    public void Timeout_SetAndGet_WorksCorrectly()
    {
        // Arrange
        var options = new RabbitMQOptions();
        const int testValue = 5000;

        // Act
        options.Timeout = testValue;

        // Assert
        Assert.Equal(testValue, options.Timeout);
    }
}
