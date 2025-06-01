namespace NetEvolve.HealthChecks.Tests.Unit.MongoDb;

using MongoDB.Bson;
using MongoDB.Driver;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.MongoDb;
using Xunit;

[TestGroup(nameof(MongoDb))]
public sealed class MongoDbOptionsTests
{
    [Fact]
    public void Constructor_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var options = new MongoDbOptions();

        // Assert
        Assert.Null(options.KeyedService);
        Assert.Equal(100, options.Timeout);
        Assert.Equal(MongoDbHealthCheck.DefaultCommandAsync, options.CommandAsync);
    }

    [Fact]
    public void KeyedService_SetAndGet_WorksCorrectly()
    {
        // Arrange
        var options = new MongoDbOptions();
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
        var options = new MongoDbOptions();
        const int testValue = 5000;

        // Act
        options.Timeout = testValue;

        // Assert
        Assert.Equal(testValue, options.Timeout);
    }

    [Fact]
    public void CommandAsync_SetAndGet_WorksCorrectly()
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
        Assert.Equal(testValue, options.CommandAsync);
    }
}
