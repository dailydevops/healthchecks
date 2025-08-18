namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Search;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Search;

[TestGroup($"{nameof(Azure)}.{nameof(Search)}")]
public class SearchIndexAvailableOptionsTests
{
    [Test]
    public void Timeout_WhenNotSet_ExpectedDefaultValue()
    {
        // Arrange
        var options = new SearchIndexAvailableOptions();

        // Act
        var result = options.Timeout;

        // Assert
        _ = await Assert.That(result).IsEqualTo(100);
    }

    [Test]
    public void Mode_WhenNotSet_ExpectedNull()
    {
        // Arrange
        var options = new SearchIndexAvailableOptions();

        // Act
        var result = options.Mode;

        // Assert
        _ = await Assert.That(result).IsNull();
    }

    [Test]
    public void IndexName_WhenNotSet_ExpectedNull()
    {
        // Arrange
        var options = new SearchIndexAvailableOptions();

        // Act
        var result = options.IndexName;

        // Assert
        _ = await Assert.That(result).IsNull();
    }
}