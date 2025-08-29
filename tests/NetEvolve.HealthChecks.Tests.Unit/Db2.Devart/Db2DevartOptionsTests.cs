namespace NetEvolve.HealthChecks.Tests.Unit.Db2.Devart;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Db2.Devart;

[TestGroup($"{nameof(Db2)}.{nameof(Devart)}")]
public sealed class Db2DevartOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new Db2DevartOptions { ConnectionString = "test" };
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}