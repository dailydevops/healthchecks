namespace NetEvolve.HealthChecks.Tests.Unit.Oracle.Devart;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Oracle.Devart;

[TestGroup($"{nameof(Oracle)}.{nameof(Devart)}")]
public sealed class OracleDevartOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new OracleDevartOptions { ConnectionString = "test" };
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
