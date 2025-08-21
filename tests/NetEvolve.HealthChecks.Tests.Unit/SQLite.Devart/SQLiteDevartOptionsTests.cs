namespace NetEvolve.HealthChecks.Tests.Unit.SQLite.Devart;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.SQLite.Devart;

[TestGroup($"{nameof(SQLite)}.{nameof(Devart)}")]
public sealed class SQLiteDevartOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new SQLiteDevartOptions { ConnectionString = "test" };
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}