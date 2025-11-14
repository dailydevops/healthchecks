namespace NetEvolve.HealthChecks.Tests.Unit.Couchbase;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Couchbase;

[TestGroup(nameof(Couchbase))]
public sealed class CouchbaseOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new CouchbaseOptions();
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
