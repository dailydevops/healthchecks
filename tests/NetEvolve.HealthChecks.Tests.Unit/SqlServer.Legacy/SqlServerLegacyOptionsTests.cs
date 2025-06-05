namespace NetEvolve.HealthChecks.Tests.Unit.SqlServerLegacy;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.SqlServer.Legacy;

[TestGroup(nameof(SqlServerLegacy))]
public sealed class SqlServerLegacyOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new SqlServerLegacyOptions { ConnectionString = "test" };
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
