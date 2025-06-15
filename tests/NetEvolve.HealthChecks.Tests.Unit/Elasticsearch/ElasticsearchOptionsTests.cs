namespace NetEvolve.HealthChecks.Tests.Unit.Elasticsearch;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Elasticsearch;

[TestGroup(nameof(Elasticsearch))]
public sealed class ElasticsearchOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new ElasticsearchOptions();
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
