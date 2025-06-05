namespace NetEvolve.HealthChecks.Tests.Unit.Apache.Kafka;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Apache.Kafka;

[TestGroup($"{nameof(Apache)}.{nameof(Kafka)}")]
public sealed class KafkaOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new KafkaOptions();
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
