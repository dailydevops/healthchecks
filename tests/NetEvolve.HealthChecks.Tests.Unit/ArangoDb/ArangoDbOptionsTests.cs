namespace NetEvolve.HealthChecks.Tests.Unit.ArangoDb;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.ArangoDb;

[TestGroup(nameof(ArangoDb))]
public sealed class ArangoDbOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new ArangoDbOptions();
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
