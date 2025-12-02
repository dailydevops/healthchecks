namespace NetEvolve.HealthChecks.Tests.Unit.KurrentDb;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.KurrentDb;

[TestGroup(nameof(KurrentDb))]
public sealed class KurrentDbOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new KurrentDbOptions();
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
