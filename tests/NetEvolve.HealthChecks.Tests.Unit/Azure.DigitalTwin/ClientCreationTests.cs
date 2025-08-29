namespace NetEvolve.HealthChecks.Tests.Unit.Azure.DigitalTwin;

using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.DigitalTwin;

[TestGroup($"{nameof(Azure)}.{nameof(DigitalTwin)}")]
public class ClientCreationTests
{
    [Test]
    public void CreateDigitalTwinsClient_InvalidMode_ThrowUnreachableException()
    {
        var options = new DigitalTwinServiceAvailableOptions { Mode = (DigitalTwinClientCreationMode)13 };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        _ = Assert.Throws<UnreachableException>(() =>
            ClientCreation.CreateDigitalTwinsClient(options, serviceProvider)
        );
    }
}