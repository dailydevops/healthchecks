namespace NetEvolve.HealthChecks.Tests.Unit.Dapr;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Dapr;

[TestGroup(nameof(Dapr))]
public sealed class DaprOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new DaprOptions();
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
