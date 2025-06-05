namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Queues;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Queues;

[TestGroup($"{nameof(Azure)}.{nameof(Queues)}")]
public sealed class QueueServiceAvailableOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new QueueServiceAvailableOptions();
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
