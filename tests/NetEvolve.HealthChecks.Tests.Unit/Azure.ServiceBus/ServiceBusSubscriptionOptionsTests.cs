namespace NetEvolve.HealthChecks.Tests.Unit.Azure.ServiceBus;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.ServiceBus;

[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}")]
[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}.Subscription")]
public sealed class ServiceBusSubscriptionOptionsTests
{
    [Test]
    public async Task Constructor_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var options = new ServiceBusSubscriptionOptions();

        // Assert

        using (Assert.Multiple())
        {
            _ = await Assert.That(options.EnablePeekMode).IsDefault();
            _ = await Assert.That(options.SubscriptionName).IsNull();
            _ = await Assert.That(options.TopicName).IsNull();
        }
    }

    [Test]
    public async Task EnablePeekMode_SetAndGet_WorksCorrectly()
    {
        // Arrange
        var options = new ServiceBusSubscriptionOptions();
        const bool testValue = true;

        // Act
        options.EnablePeekMode = testValue;

        // Assert
        _ = await Assert.That(options.EnablePeekMode).IsEqualTo(testValue);
    }

    [Test]
    public async Task SubscriptionName_SetAndGet_WorksCorrectly()
    {
        // Arrange
        var options = new ServiceBusSubscriptionOptions();
        const string testValue = "test-subscription-name";

        // Act
        options.SubscriptionName = testValue;

        // Assert
        _ = await Assert.That(options.SubscriptionName).IsEqualTo(testValue);
    }

    [Test]
    public async Task TopicName_SetAndGet_WorksCorrectly()
    {
        // Arrange
        var options = new ServiceBusSubscriptionOptions();
        const string testValue = "test-topic";

        // Act
        options.TopicName = testValue;

        // Assert
        _ = await Assert.That(options.TopicName).IsEqualTo(testValue);
    }

    [Test]
    public async Task Instance_CanBeShallowCopied()
    {
        var options = new ServiceBusSubscriptionOptions();
        var options2 = options with { };

        using (Assert.Multiple())
        {
            _ = await Assert.That(options2).IsNotNull();
            _ = await Assert.That(options2).IsNotSameReferenceAs(options);
            _ = await Assert.That(options2.EnablePeekMode).IsEqualTo(options.EnablePeekMode);
            _ = await Assert.That(options2.SubscriptionName).IsEqualTo(options.SubscriptionName);
            _ = await Assert.That(options2.TopicName).IsEqualTo(options.TopicName);
        }
    }
}
