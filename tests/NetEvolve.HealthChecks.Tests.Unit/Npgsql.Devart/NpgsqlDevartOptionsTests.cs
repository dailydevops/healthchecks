namespace NetEvolve.HealthChecks.Tests.Unit.Npgsql.Devart;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Npgsql.Devart;

[TestGroup($"{nameof(Npgsql)}.{nameof(Devart)}")]
public sealed class NpgsqlDevartOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new NpgsqlDevartOptions { ConnectionString = "test" };
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
