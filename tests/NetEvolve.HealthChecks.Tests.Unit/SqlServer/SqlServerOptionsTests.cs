namespace NetEvolve.HealthChecks.Tests.Unit.SqlServer;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.SqlServer;

[TestGroup(nameof(SqlServer))]
public sealed class SqlServerOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new SqlServerOptions { ConnectionString = "test" };
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
