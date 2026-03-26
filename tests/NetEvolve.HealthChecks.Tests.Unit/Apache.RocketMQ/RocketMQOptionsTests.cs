namespace NetEvolve.HealthChecks.Tests.Unit.Apache.RocketMQ;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Apache.RocketMQ;

[TestGroup($"{nameof(Apache)}.{nameof(RocketMQ)}")]
public sealed class RocketMQOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new RocketMQOptions();
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }

    [Test]
    public async Task Options_AccessKey_NullWhenWhitespace()
    {
        var options = new RocketMQOptions { AccessKey = "   " };

        _ = await Assert.That(options.AccessKey).IsNull();
    }

    [Test]
    public async Task Options_AccessKey_ReturnsValueWhenSet()
    {
        var options = new RocketMQOptions { AccessKey = "my-key" };

        _ = await Assert.That(options.AccessKey).IsEqualTo("my-key");
    }

    [Test]
    public async Task Options_AccessSecret_NullWhenWhitespace()
    {
        var options = new RocketMQOptions { AccessSecret = "   " };

        _ = await Assert.That(options.AccessSecret).IsNull();
    }

    [Test]
    public async Task Options_AccessSecret_ReturnsValueWhenSet()
    {
        var options = new RocketMQOptions { AccessSecret = "my-secret" };

        _ = await Assert.That(options.AccessSecret).IsEqualTo("my-secret");
    }
}
