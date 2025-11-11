namespace NetEvolve.HealthChecks.Tests.Unit.CockroachDb;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.CockroachDb;

[TestGroup(nameof(CockroachDb))]
public sealed class CockroachDbOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new CockroachDbOptions { ConnectionString = "test" };
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
