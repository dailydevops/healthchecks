namespace NetEvolve.HealthChecks.Tests.Unit.SqlServer.Devart;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.SqlServer.Devart;

[TestGroup($"{nameof(SqlServer)}.{nameof(Devart)}")]
public sealed class SqlServerDevartOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new SqlServerDevartOptions { ConnectionString = "test" };
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
