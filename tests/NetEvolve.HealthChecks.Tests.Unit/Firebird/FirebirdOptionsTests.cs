namespace NetEvolve.HealthChecks.Tests.Unit.Firebird;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Firebird;

[TestGroup(nameof(Firebird))]
public sealed class FirebirdOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new FirebirdOptions { ConnectionString = "test" };
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
