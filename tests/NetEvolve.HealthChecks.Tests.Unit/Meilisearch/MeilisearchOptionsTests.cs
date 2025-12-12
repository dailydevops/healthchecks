namespace NetEvolve.HealthChecks.Tests.Unit.Meilisearch;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Meilisearch;

[TestGroup(nameof(Meilisearch))]
public sealed class MeilisearchOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new MeilisearchOptions
        {
            Host = "http://localhost:7700",
            ApiKey = "test-key",
            Mode = MeilisearchClientCreationMode.Internal,
        };
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
