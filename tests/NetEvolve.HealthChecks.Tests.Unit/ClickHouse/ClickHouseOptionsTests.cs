namespace NetEvolve.HealthChecks.Tests.Unit.ClickHouse;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.ClickHouse;

[TestGroup(nameof(ClickHouse))]
public sealed class ClickHouseOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new ClickHouseOptions { ConnectionString = "test" };
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
