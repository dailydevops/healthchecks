namespace NetEvolve.HealthChecks.Tests.Unit.Oracle;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Oracle;

[TestGroup(nameof(Oracle))]
public sealed class OracleOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new OracleOptions { ConnectionString = "test" };
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
