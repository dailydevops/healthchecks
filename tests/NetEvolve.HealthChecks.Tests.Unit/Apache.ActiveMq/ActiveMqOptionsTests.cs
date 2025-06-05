namespace NetEvolve.HealthChecks.Tests.Unit.Apache.ActiveMq;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Apache.ActiveMq;

[TestGroup($"{nameof(Apache)}.{nameof(ActiveMq)}")]
public sealed class ActiveMqOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new ActiveMqOptions();
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
