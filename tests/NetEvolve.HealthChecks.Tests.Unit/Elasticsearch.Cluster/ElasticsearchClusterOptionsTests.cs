namespace NetEvolve.HealthChecks.Tests.Unit.Elasticsearch.Cluster;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Elasticsearch.Cluster;

[TestGroup($"{nameof(Elasticsearch)}.{nameof(Cluster)}")]
public sealed class ElasticsearchClusterOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new ElasticsearchClusterOptions();
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
