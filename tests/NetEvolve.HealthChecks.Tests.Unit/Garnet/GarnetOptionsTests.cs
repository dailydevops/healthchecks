namespace NetEvolve.HealthChecks.Tests.Unit.Garnet;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Garnet;

[TestGroup(nameof(Garnet))]
public sealed class GarnetOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new GarnetOptions
        {
            Hostname = "test",
            Port = 1234,
            Mode = ConnectionHandleMode.Create,
        };
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
