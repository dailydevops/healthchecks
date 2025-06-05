namespace NetEvolve.HealthChecks.Tests.Unit.RabbitMQ;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.RabbitMQ;

[TestGroup(nameof(RabbitMQ))]
public sealed class RabbitMQOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new RabbitMQOptions();
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
