namespace NetEvolve.HealthChecks.Tests.Unit.Odbc;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Odbc;

[TestGroup(nameof(Odbc))]
public sealed class OdbcOptionsTests
{
    [Test]
    public async Task Options_NotTheSame_Expected()
    {
        var options1 = new OdbcOptions { ConnectionString = "test" };
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
