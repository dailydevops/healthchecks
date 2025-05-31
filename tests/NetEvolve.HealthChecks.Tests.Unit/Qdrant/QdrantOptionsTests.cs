namespace NetEvolve.HealthChecks.Tests.Unit.Qdrant;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Qdrant;

[TestGroup(nameof(Qdrant))]
public sealed class QdrantOptionsTests
{
    [Test]
    public async Task Constructor_WhenCalled_SetsDefaultTimeout()
    {
        // Arrange & Act
        var options = new QdrantOptions();

        // Assert
        _ = await Assert.That(options.Timeout).IsEqualTo(100);
    }

    [Test]
    public async Task KeyedService_WhenSetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var options = new QdrantOptions();
        const string expectedValue = "TestKeyedService";

        // Act
        options.KeyedService = expectedValue;

        // Assert
        _ = await Assert.That(options.KeyedService).IsEqualTo(expectedValue);
    }

    [Test]
    public async Task Timeout_WhenSetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var options = new QdrantOptions();
        const int expectedValue = 500;

        // Act
        options.Timeout = expectedValue;

        // Assert
        _ = await Assert.That(options.Timeout).IsEqualTo(expectedValue);
    }

    [Test]
    public async Task Equals_WhenSameValues_ReturnsTrue()
    {
        // Arrange
        var options1 = new QdrantOptions { KeyedService = "TestService", Timeout = 200 };
        var options2 = new QdrantOptions { KeyedService = "TestService", Timeout = 200 };

        // Act & Assert
        _ = await Assert.That(options2).IsEqualTo(options1);
    }

    [Test]
    public async Task Equals_WhenDifferentValues_ReturnsFalse()
    {
        // Arrange
        var options1 = new QdrantOptions { KeyedService = "TestService1", Timeout = 200 };
        var options2 = new QdrantOptions { KeyedService = "TestService2", Timeout = 200 };

        // Act & Assert
        _ = await Assert.That(options1).IsNotEqualTo(options2);
    }

    [Test]
    public async Task Clone_WhenCalled_ReturnsCopyWithSameValues()
    {
        // Arrange
        var original = new QdrantOptions { KeyedService = "TestService", Timeout = 250 };

        // Act
        var clone = original with
        { };

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(clone).IsEqualTo(original);
            _ = await Assert.That(clone).IsNotSameReferenceAs(original);
        }
    }
}
