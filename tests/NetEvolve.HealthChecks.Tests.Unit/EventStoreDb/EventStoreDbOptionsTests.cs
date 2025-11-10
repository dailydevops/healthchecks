namespace NetEvolve.HealthChecks.Tests.Unit.EventStoreDb;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.EventStoreDb;

[TestGroup(nameof(EventStoreDb))]
public sealed class EventStoreDbOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new EventStoreDbOptions();
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
