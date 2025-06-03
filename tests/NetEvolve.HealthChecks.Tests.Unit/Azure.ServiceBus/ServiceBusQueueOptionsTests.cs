namespace NetEvolve.HealthChecks.Tests.Unit.Azure.ServiceBus;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.ServiceBus;

[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}")]
[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}.Queue")]
public sealed class ServiceBusQueueOptionsTests
{
    [Test]
    public async Task Constructor_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var options = new ServiceBusQueueOptions();

        // Assert

        using (Assert.Multiple())
        {
            _ = await Assert.That(options.EnablePeekMode).IsDefault();
            _ = await Assert.That(options.QueueName).IsNull();
        }
    }

    [Test]
    public async Task EnablePeekMode_SetAndGet_WorksCorrectly()
    {
        // Arrange
        var options = new ServiceBusQueueOptions();
        const bool testValue = true;

        // Act
        options.EnablePeekMode = testValue;

        // Assert
        _ = await Assert.That(options.EnablePeekMode).IsEqualTo(testValue);
    }

    [Test]
    public async Task QueueName_SetAndGet_WorksCorrectly()
    {
        // Arrange
        var options = new ServiceBusQueueOptions();
        const string testValue = "test";

        // Act
        options.QueueName = testValue;

        // Assert
        _ = await Assert.That(options.QueueName).IsEqualTo(testValue);
    }

    [Test]
    public async Task Instance_CanBeShallowCopied()
    {
        var options = new ServiceBusQueueOptions();
        var options2 = options with { };

        using (Assert.Multiple())
        {
            _ = await Assert.That(options2).IsNotNull();
            _ = await Assert.That(options2).IsNotSameReferenceAs(options);
            _ = await Assert.That(options2.EnablePeekMode).IsEqualTo(options.EnablePeekMode);
            _ = await Assert.That(options2.QueueName).IsEqualTo(options.QueueName);
        }
    }
}
