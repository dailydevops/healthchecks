namespace NetEvolve.HealthChecks.Tests.Unit.Azure.ServiceBus;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.ServiceBus;

[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}")]
[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}.Topic")]
public sealed class ServiceBusTopicOptionsTests
{
    [Test]
    public async Task Constructor_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var options = new ServiceBusTopicOptions();

        // Assert
        _ = await Assert.That(options.TopicName).IsNull();
    }

    [Test]
    public async Task TopicName_SetAndGet_WorksCorrectly()
    {
        // Arrange
        var options = new ServiceBusTopicOptions();
        const string testValue = "test-keyed-service";

        // Act
        options.TopicName = testValue;

        // Assert
        _ = await Assert.That(options.TopicName).IsEqualTo(testValue);
    }

    [Test]
    public async Task Instance_CanBeShallowCopied()
    {
        var options = new ServiceBusTopicOptions();
        var options2 = options with { };

        using (Assert.Multiple())
        {
            _ = await Assert.That(options2).IsNotNull();
            _ = await Assert.That(options2).IsNotSameReferenceAs(options);
            _ = await Assert.That(options2.TopicName).IsEqualTo(options.TopicName);
        }
    }
}
