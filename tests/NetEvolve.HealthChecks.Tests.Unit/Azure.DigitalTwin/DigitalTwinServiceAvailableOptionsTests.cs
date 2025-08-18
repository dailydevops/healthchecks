namespace NetEvolve.HealthChecks.Tests.Unit.Azure.DigitalTwin;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.DigitalTwin;

[TestGroup($"{nameof(Azure)}.{nameof(DigitalTwin)}")]
public sealed class DigitalTwinServiceAvailableOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new DigitalTwinServiceAvailableOptions();
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}