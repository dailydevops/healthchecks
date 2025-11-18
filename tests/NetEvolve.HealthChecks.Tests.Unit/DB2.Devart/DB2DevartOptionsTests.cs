namespace NetEvolve.HealthChecks.Tests.Unit.DB2.Devart;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.DB2.Devart;

[TestGroup($"{nameof(DB2)}.{nameof(Devart)}")]
public sealed class DB2DevartOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new DB2DevartOptions { ConnectionString = "test" };
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
