namespace NetEvolve.HealthChecks.Tests.Unit.Ollama;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Ollama;

[TestGroup(nameof(Ollama))]
public sealed class OllamaOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new OllamaOptions { Uri = new System.Uri("http://localhost:11434"), Timeout = 5000 };
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
