namespace NetEvolve.HealthChecks.Tests.Unit.MySql.Devart;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.MySql.Devart;

[TestGroup($"{nameof(MySql)}.{nameof(Devart)}")]
public sealed class MySqlDevartOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new MySqlDevartOptions { ConnectionString = "test" };
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}