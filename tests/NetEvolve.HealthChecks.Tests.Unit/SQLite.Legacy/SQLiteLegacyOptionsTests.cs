namespace NetEvolve.HealthChecks.Tests.Unit.SQLite.Legacy;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.SQLite.Legacy;

[TestGroup($"{nameof(SQLite)}.{nameof(Legacy)}")]
public sealed class SQLiteLegacyOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new SQLiteLegacyOptions { ConnectionString = "Test" };
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
