namespace NetEvolve.HealthChecks.Tests.Unit.RavenDb;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.RavenDb;
using Raven.Client.Documents;

[TestGroup(nameof(RavenDb))]
public sealed class MongoDbOptionsTests
{
    [Test]
    public async Task Constructor_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var options = new RavenDbOptions();

        // Assert

        using (Assert.Multiple())
        {
            _ = await Assert.That(options.KeyedService).IsNull();
            _ = await Assert.That(options.Timeout).IsEqualTo(100);
            _ = await Assert.That(options.CommandAsync).IsEqualTo(RavenDbHealthCheck.DefaultCommandAsync);
        }
    }

    [Test]
    public async Task KeyedService_SetAndGet_WorksCorrectly()
    {
        // Arrange
        var options = new RavenDbOptions();
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
        var options = new RavenDbOptions();
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
        var options = new RavenDbOptions();
        static async Task<bool> testValue(IDocumentStore _1, CancellationToken _2)
        {
            await Task.CompletedTask;
            return true;
        }

        // Act
        options.CommandAsync = testValue;

        // Assert
        _ = await Assert.That(options.CommandAsync).IsEqualTo(testValue);
    }
}
