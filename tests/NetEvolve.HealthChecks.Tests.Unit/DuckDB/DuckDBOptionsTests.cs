namespace NetEvolve.HealthChecks.Tests.Unit.DuckDB;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.DuckDB;

[TestGroup(nameof(DuckDB))]
public sealed class DuckDBOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new DuckDBOptions { ConnectionString = "Test" };
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
