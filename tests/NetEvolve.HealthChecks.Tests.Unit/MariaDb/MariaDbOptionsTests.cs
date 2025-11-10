namespace NetEvolve.HealthChecks.Tests.Unit.MariaDb;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.MariaDb;

[TestGroup(nameof(MariaDb))]
public sealed class MariaDbOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new MariaDbOptions { ConnectionString = "test" };
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
