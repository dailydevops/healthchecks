namespace NetEvolve.HealthChecks.Tests.Unit.InfluxDB;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.InfluxDB;

[TestGroup(nameof(InfluxDB))]
public sealed class InfluxDBOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new InfluxDBOptions();
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
