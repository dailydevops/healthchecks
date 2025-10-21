namespace NetEvolve.HealthChecks.Tests.Unit.Consul;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Consul;

[TestGroup(nameof(Consul))]
public sealed class ConsulOptionsTests
{
    [Test]
    public async Task Ctor_Default_ExpectedValues()
    {
        // Arrange / Act
        var options = new ConsulOptions();

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(options.Timeout).IsEqualTo(100);
            _ = await Assert.That(options.KeyedService).IsNull();
        }
    }
}
