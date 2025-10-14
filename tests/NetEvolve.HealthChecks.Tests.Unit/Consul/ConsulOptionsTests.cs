namespace NetEvolve.HealthChecks.Tests.Unit.Consul;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Consul;

[TestGroup(nameof(Consul))]
public sealed class ConsulOptionsTests
{
    [Test]
    public void Ctor_Default_ExpectedValues()
    {
        // Arrange / Act
        var options = new ConsulOptions();

        // Assert
        using (Assert.Multiple())
        {
            _ = Assert.That(options.Timeout).IsEqualTo(100);
            _ = Assert.That(options.KeyedService).IsNull();
        }
    }
}
