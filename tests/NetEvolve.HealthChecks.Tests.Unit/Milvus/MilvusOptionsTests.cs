namespace NetEvolve.HealthChecks.Tests.Unit.Milvus;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Milvus;

[TestGroup(nameof(Milvus))]
public sealed class MilvusOptionsTests
{
    [Test]
    public async Task Timeout_DefaultValue_Expected()
    {
        // Arrange
        var options = new MilvusOptions();

        // Act
        var timeout = options.Timeout;

        // Assert
        _ = await Assert.That(timeout).IsEqualTo(100);
    }

    [Test]
    public async Task KeyedService_DefaultValue_Expected()
    {
        // Arrange
        var options = new MilvusOptions();

        // Act
        var keyedService = options.KeyedService;

        // Assert
        _ = await Assert.That(keyedService).IsNull();
    }
}
