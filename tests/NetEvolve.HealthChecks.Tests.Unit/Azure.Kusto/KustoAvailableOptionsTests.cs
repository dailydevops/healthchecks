namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Kusto;

using System;
using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Kusto;

[TestGroup($"{nameof(Azure)}.{nameof(Kusto)}")]
public sealed class KustoAvailableOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new KustoAvailableOptions();
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
