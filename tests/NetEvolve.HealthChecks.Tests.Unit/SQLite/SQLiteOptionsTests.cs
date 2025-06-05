namespace NetEvolve.HealthChecks.Tests.Unit.SQLite;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.SQLite;

[TestGroup(nameof(SQLite))]
public sealed class SQLiteOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new SQLiteOptions { ConnectionString = "test" };
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
