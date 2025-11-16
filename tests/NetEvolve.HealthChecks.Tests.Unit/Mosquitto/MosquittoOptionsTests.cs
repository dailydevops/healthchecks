namespace NetEvolve.HealthChecks.Tests.Unit.Mosquitto;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Mosquitto;

[TestGroup(nameof(Mosquitto))]
public sealed class MosquittoOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new MosquittoOptions();
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
