namespace NetEvolve.HealthChecks.Tests.Unit.Http;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Http;

[TestGroup(nameof(Http))]
public sealed class HttpOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new HttpOptions();
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }

    [Test]
    public async Task Options_WithDifferentUri_NotEqual()
    {
        var options1 = new HttpOptions { Uri = "https://example.com" };
        var options2 = new HttpOptions { Uri = "https://different.com" };

        _ = await Assert.That(options1).IsNotEqualTo(options2);
    }

    [Test]
    public async Task Options_DefaultValues_Expected()
    {
        var options = new HttpOptions();

        _ = await Assert
            .That(options.HttpMethod)
            .IsEqualTo("GET")
            .And.That(options.ExpectedHttpStatusCodes)
            .ContainsValue(200)
            .And.That(options.Timeout)
            .IsEqualTo(5000)
            .And.That(options.ContentType)
            .IsEqualTo("application/json")
            .And.That(options.AllowAutoRedirect)
            .IsTrue();
    }
}
