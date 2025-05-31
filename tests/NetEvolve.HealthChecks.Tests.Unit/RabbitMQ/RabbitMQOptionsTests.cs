namespace NetEvolve.HealthChecks.Tests.Unit.RabbitMQ;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.RabbitMQ;

[TestGroup(nameof(RabbitMQ))]
public sealed class RabbitMQOptionsTests
{
    [Test]
    public async Task Constructor_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var options = new RabbitMQOptions();

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(options.KeyedService).IsNull();
            _ = await Assert.That(options.Timeout).IsEqualTo(100);
        }
    }

    [Test]
    public async Task KeyedService_SetAndGet_WorksCorrectly()
    {
        // Arrange
        var options = new RabbitMQOptions();
        const string testValue = "test-keyed-service";

        // Act
        options.KeyedService = testValue;

        // Assert
        _ = await Assert.That(options.KeyedService).IsEqualTo(testValue);
    }

    [Test]
    public async Task Timeout_SetAndGet_WorksCorrectly()
    {
        // Arrange
        var options = new RabbitMQOptions();
        const int testValue = 5000;

        // Act
        options.Timeout = testValue;

        // Assert
        _ = await Assert.That(options.Timeout).IsEqualTo(testValue);
    }
}
