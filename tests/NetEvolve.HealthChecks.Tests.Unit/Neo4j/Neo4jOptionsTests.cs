namespace NetEvolve.HealthChecks.Tests.Unit.Neo4j;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Neo4j;

[TestGroup(nameof(Neo4j))]
public sealed class Neo4jOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new Neo4jOptions();
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
