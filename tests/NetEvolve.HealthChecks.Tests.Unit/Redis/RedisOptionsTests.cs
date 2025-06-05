namespace NetEvolve.HealthChecks.Tests.Unit.Redis;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Redis;

[TestGroup(nameof(Redis))]
public sealed class RedisOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new RedisOptions { ConnectionString = "test", Mode = ConnectionHandleMode.Create };
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
