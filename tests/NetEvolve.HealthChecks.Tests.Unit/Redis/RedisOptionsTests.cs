namespace NetEvolve.HealthChecks.Tests.Unit.Redis;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Redis;

[TestGroup(nameof(Redis))]
public sealed class RedisOptionsTests
{
    [Test]
    public async Task Constructor_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var options = new RedisOptions();

        // Assert

        using (Assert.Multiple())
        {
            _ = await Assert.That(options.ConnectionString).IsNull();
            _ = await Assert.That(options.Timeout).IsEqualTo(100);
            _ = await Assert.That(options.Mode).IsDefault();
        }
    }

    [Test]
    public async Task ConnectionString_SetAndGet_WorksCorrectly()
    {
        // Arrange
        var options = new RedisOptions();
        const string testValue = "test-connection-string";

        // Act
        options.ConnectionString = testValue;

        // Assert
        _ = await Assert.That(options.ConnectionString).IsEqualTo(testValue);
    }

    [Test]
    public async Task Timeout_SetAndGet_WorksCorrectly()
    {
        // Arrange
        var options = new RedisOptions();
        const int testValue = 5000;

        // Act
        options.Timeout = testValue;

        // Assert
        _ = await Assert.That(options.Timeout).IsEqualTo(testValue);
    }

    [Test]
    public async Task Mode_SetAndGet_WorksCorrectly()
    {
        // Arrange
        var options = new RedisOptions();
        const ConnectionHandleMode testValue = ConnectionHandleMode.Create;

        // Act
        options.Mode = testValue;

        // Assert
        _ = await Assert.That(options.Mode).IsEqualTo(testValue);
    }

    [Test]
    public async Task Instance_CanBeShallowCopied()
    {
        var options = new RedisOptions();
        var options2 = options with { };

        using (Assert.Multiple())
        {
            _ = await Assert.That(options2).IsNotNull();
            _ = await Assert.That(options2).IsNotSameReferenceAs(options);
            _ = await Assert.That(options2.ConnectionString).IsEqualTo(options.ConnectionString);
            _ = await Assert.That(options2.Timeout).IsEqualTo(options.Timeout);
            _ = await Assert.That(options2.Mode).IsEqualTo(options.Mode);
        }
    }
}
