namespace NetEvolve.HealthChecks.Tests.Unit.Azure.ServiceBus;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.ServiceBus;

[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}")]
public sealed class ServiceBusOptionsBaseTests
{
    private sealed record BaseImplementation : ServiceBusOptionsBase { }

    [Test]
    public async Task Instance_CanBeShallowCopied()
    {
        var options = new BaseImplementation();
        var options2 = options with { };

        using (Assert.Multiple())
        {
            _ = await Assert.That(options2).IsNotNull();
            _ = await Assert.That(options2).IsNotSameReferenceAs(options);
            _ = await Assert.That(options2.KeyedService).IsEqualTo(options.KeyedService);
            _ = await Assert.That(options2.Mode).IsEqualTo(options.Mode);
            _ = await Assert.That(options2.ConnectionString).IsEqualTo(options.ConnectionString);
            _ = await Assert.That(options2.FullyQualifiedNamespace).IsEqualTo(options.FullyQualifiedNamespace);
            _ = await Assert.That(options2.Timeout).IsEqualTo(options.Timeout);
        }
    }
}
