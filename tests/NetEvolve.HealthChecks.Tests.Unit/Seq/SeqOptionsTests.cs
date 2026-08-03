namespace NetEvolve.HealthChecks.Tests.Unit.Seq;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Seq;

[TestGroup(nameof(Seq))]
public sealed class SeqOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new SeqOptions();
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
