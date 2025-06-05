namespace NetEvolve.HealthChecks.Tests.Unit.DB2;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.DB2;

[TestGroup(nameof(DB2))]
public sealed class DB2OptionsTests
{
    [Test]
    public async Task Options_NotTheSame_Expected()
    {
        var options1 = new DB2Options { ConnectionString = "Test" };
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
