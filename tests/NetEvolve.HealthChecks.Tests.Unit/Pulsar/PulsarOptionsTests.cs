namespace NetEvolve.HealthChecks.Tests.Unit.Pulsar;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Pulsar;

[TestGroup(nameof(Pulsar))]
public sealed class PulsarOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new PulsarOptions();
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
