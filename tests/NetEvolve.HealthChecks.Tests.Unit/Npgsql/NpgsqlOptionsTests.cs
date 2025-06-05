namespace NetEvolve.HealthChecks.Tests.Unit.Npgsql;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Npgsql;

[TestGroup(nameof(Npgsql))]
public sealed class NpgsqlOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new NpgsqlOptions { ConnectionString = "test" };
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
