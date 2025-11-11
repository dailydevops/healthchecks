namespace NetEvolve.HealthChecks.Tests.Unit.GCP.Bigtable;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.GCP.Bigtable;

[TestGroup($"GCP.{nameof(Bigtable)}")]
public sealed class BigtableOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new BigtableOptions { Timeout = 100 };
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }

    [Test]
    public async Task Options_DefaultTimeout_Is100()
    {
        var options = new BigtableOptions();

        _ = await Assert.That(options.Timeout).IsEqualTo(100);
    }

    [Test]
    public async Task Options_WithCustomTimeout_TimeoutSet()
    {
        var options = new BigtableOptions { Timeout = 5000 };

        _ = await Assert.That(options.Timeout).IsEqualTo(5000);
    }

    [Test]
    public async Task Options_WithKeyedService_KeyedServiceSet()
    {
        const string keyedService = "my-bigtable";
        var options = new BigtableOptions { KeyedService = keyedService };

        _ = await Assert.That(options.KeyedService).IsEqualTo(keyedService);
    }

    [Test]
    public async Task Options_WithClone_AllPropertiesCloned()
    {
        var options1 = new BigtableOptions { Timeout = 5000, KeyedService = "test" };
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
        var options1 = new BigtableOptions { Timeout = 100 };
        var options2 = new BigtableOptions { Timeout = 200 };

        _ = await Assert.That(options1).IsNotEqualTo(options2);
    }
}
