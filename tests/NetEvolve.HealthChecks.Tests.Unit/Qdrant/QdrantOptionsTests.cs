namespace NetEvolve.HealthChecks.Tests.Unit.Qdrant;

using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Qdrant;
using Xunit;

[TestGroup(nameof(Qdrant))]
public sealed class QdrantOptionsTests
{
    [Fact]
    public void Constructor_WhenCalled_SetsDefaultTimeout()
    {
        // Arrange & Act
        var options = new QdrantOptions();

        // Assert
        Assert.Equal(100, options.Timeout);
    }

    [Fact]
    public void KeyedService_WhenSetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var options = new QdrantOptions();
        const string expectedValue = "TestKeyedService";

        // Act
        options.KeyedService = expectedValue;

        // Assert
        Assert.Equal(expectedValue, options.KeyedService);
    }

    [Fact]
    public void Timeout_WhenSetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var options = new QdrantOptions();
        const int expectedValue = 500;

        // Act
        options.Timeout = expectedValue;

        // Assert
        Assert.Equal(expectedValue, options.Timeout);
    }

    [Fact]
    public void Equals_WhenSameValues_ReturnsTrue()
    {
        // Arrange
        var options1 = new QdrantOptions { KeyedService = "TestService", Timeout = 200 };
        var options2 = new QdrantOptions { KeyedService = "TestService", Timeout = 200 };

        // Act & Assert
        Assert.Equal(options1, options2);
    }

    [Fact]
    public void Equals_WhenDifferentValues_ReturnsFalse()
    {
        // Arrange
        var options1 = new QdrantOptions { KeyedService = "TestService1", Timeout = 200 };
        var options2 = new QdrantOptions { KeyedService = "TestService2", Timeout = 200 };

        // Act & Assert
        Assert.NotEqual(options1, options2);
    }

    [Fact]
    public void Clone_WhenCalled_ReturnsCopyWithSameValues()
    {
        // Arrange
        var original = new QdrantOptions { KeyedService = "TestService", Timeout = 250 };

        // Act
        var clone = original with
        { };

        // Assert
        Assert.Equal(original, clone);
        Assert.NotSame(original, clone);
    }
}
