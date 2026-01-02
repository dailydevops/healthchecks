namespace NetEvolve.HealthChecks.Tests.Unit.IbmMQ;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.IbmMQ;

[TestGroup(nameof(IbmMQ))]
public sealed class IbmMQOptionsTests
{
    [Test]
    public async Task DefaultTimeout_ShouldBe100()
    {
        // Arrange & Act
        var options = new IbmMQOptions();

        // Assert
        _ = await Assert.That(options.Timeout).IsEqualTo(100);
    }

    [Test]
    public async Task KeyedService_DefaultNull()
    {
        // Arrange & Act
        var options = new IbmMQOptions();

        // Assert
        _ = await Assert.That(options.KeyedService).IsNull();
    }

    [Test]
    public async Task Timeout_CanBeSet()
    {
        // Arrange
        var options = new IbmMQOptions { Timeout = 500 };

        // Assert
        _ = await Assert.That(options.Timeout).IsEqualTo(500);
    }

    [Test]
    public async Task KeyedService_CanBeSet()
    {
        // Arrange
        var options = new IbmMQOptions { KeyedService = "test-key" };

        // Assert
        _ = await Assert.That(options.KeyedService).IsEqualTo("test-key");
    }
}
