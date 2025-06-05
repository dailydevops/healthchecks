namespace NetEvolve.HealthChecks.Tests.Unit.Qdrant;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Qdrant;

[TestGroup(nameof(Qdrant))]
public sealed class QdrantOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new QdrantOptions();
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
