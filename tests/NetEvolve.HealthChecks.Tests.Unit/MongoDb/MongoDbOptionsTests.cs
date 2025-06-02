namespace NetEvolve.HealthChecks.Tests.Unit.MongoDb;

using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.MongoDb;

[TestGroup(nameof(MongoDb))]
public sealed class MongoDbOptionsTests
{
    [Test]
    public async Task Constructor_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var options = new MongoDbOptions();

        // Assert

        using (Assert.Multiple())
        {
            _ = await Assert.That(options.KeyedService).IsNull();
            _ = await Assert.That(options.Timeout).IsEqualTo(100);
            _ = await Assert.That(options.CommandAsync).IsEqualTo(MongoDbHealthCheck.DefaultCommandAsync);
        }
    }

    [Test]
    public async Task KeyedService_SetAndGet_WorksCorrectly()
    {
        // Arrange
        var options = new MongoDbOptions();
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
        var options = new MongoDbOptions();
        const int testValue = 5000;

        // Act
        options.Timeout = testValue;

        // Assert
        _ = await Assert.That(options.Timeout).IsEqualTo(testValue);
    }

    [Test]
    public async Task CommandAsync_SetAndGet_WorksCorrectly()
    {
        // Arrange
        var options = new MongoDbOptions();
        static async Task<BsonDocument> testValue(MongoClient _, CancellationToken cancellationToken)
        {
            await Task.Delay(0, cancellationToken);
            return new BsonDocument("test", 1);
        }

        // Act
        options.CommandAsync = testValue;

        // Assert
        _ = await Assert.That(options.CommandAsync).IsEqualTo(testValue);
    }

    [Test]
    public async Task Instance_CanBeShallowCopied()
    {
        var options = new MongoDbOptions();
        var options2 = options with { };

        using (Assert.Multiple())
        {
            _ = await Assert.That(options2).IsNotNull();
            _ = await Assert.That(options2).IsNotSameReferenceAs(options);
            _ = await Assert.That(options2.KeyedService).IsEqualTo(options.KeyedService);
            _ = await Assert.That(options2.Timeout).IsEqualTo(options.Timeout);
            _ = await Assert.That(options2.CommandAsync).IsEqualTo(options.CommandAsync);
        }
    }
}
