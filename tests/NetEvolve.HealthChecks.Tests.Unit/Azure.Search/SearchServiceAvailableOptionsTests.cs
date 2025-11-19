namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Search;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Search;

[TestGroup($"{nameof(Azure)}.{nameof(Search)}")]
public sealed class SearchServiceAvailableOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new SearchServiceAvailableOptions();
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
