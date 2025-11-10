namespace NetEvolve.HealthChecks.Tests.Unit.GCP.PubSub;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.GCP.PubSub;

[TestGroup($"GCP.{nameof(PubSub)}")]
public sealed class PubSubOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new PubSubOptions { Timeout = 100 };
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }

    [Test]
    public async Task Options_DefaultTimeout_Is100()
    {
        var options = new PubSubOptions();

        _ = await Assert.That(options.Timeout).IsEqualTo(100);
    }

    [Test]
    public async Task Options_WithCustomTimeout_TimeoutSet()
    {
        var options = new PubSubOptions { Timeout = 5000 };

        _ = await Assert.That(options.Timeout).IsEqualTo(5000);
    }

    [Test]
    public async Task Options_WithKeyedService_KeyedServiceSet()
    {
        const string keyedService = "my-pubsub";
        var options = new PubSubOptions { KeyedService = keyedService };

        _ = await Assert.That(options.KeyedService).IsEqualTo(keyedService);
    }

    [Test]
    public async Task Options_WithClone_AllPropertiesCloned()
    {
        var options1 = new PubSubOptions { Timeout = 5000, KeyedService = "test" };
        var options2 = options1 with { };

        using (Assert.Multiple())
        {
            _ = await Assert.That(options2.Timeout).IsEqualTo(options1.Timeout);
            _ = await Assert.That(options2.KeyedService).IsEqualTo(options1.KeyedService);
        }
    }

    [Test]
    public async Task Options_WithDifferentValues_NotEqual()
    {
        var options1 = new PubSubOptions { Timeout = 100 };
        var options2 = new PubSubOptions { Timeout = 200 };

        _ = await Assert.That(options1).IsNotEqualTo(options2);
    }
}
