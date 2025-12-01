namespace NetEvolve.HealthChecks.Tests.Unit.Kubernetes;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Kubernetes;

[TestGroup(nameof(Kubernetes))]
public sealed class KubernetesOptionsTests
{
    [Test]
    public async Task Ctor_Default_ExpectedValues()
    {
        // Arrange / Act
        var options = new KubernetesOptions();

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(options.Timeout).IsEqualTo(100);
            _ = await Assert.That(options.KeyedService).IsNull();
        }
    }
}
