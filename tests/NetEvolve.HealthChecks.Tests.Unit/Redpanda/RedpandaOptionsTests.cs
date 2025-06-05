namespace NetEvolve.HealthChecks.Tests.Unit.Redpanda;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Redpanda;

[TestGroup(nameof(Redpanda))]
public sealed class RedpandaOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new RedpandaOptions { Mode = ProducerHandleMode.Create };
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
