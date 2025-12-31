namespace NetEvolve.HealthChecks.Tests.Unit.Azure.CosmosDB;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.CosmosDB;

[TestGroup($"{nameof(Azure)}.{nameof(CosmosDB)}")]
public sealed class CosmosDbAvailableOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new CosmosDbAvailableOptions();
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
