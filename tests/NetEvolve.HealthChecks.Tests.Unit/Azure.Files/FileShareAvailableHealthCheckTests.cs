namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Files;

using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Files;

[TestGroup($"{nameof(Azure)}.{nameof(Files)}")]
public class ClientCreationTests
{
    [Test]
    public void CreateShareServiceClient_InvalidMode_ThrowUnreachableException()
    {
        var options = new FileShareAvailableOptions { Mode = (FileClientCreationMode)13 };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        _ = Assert.Throws<UnreachableException>(() => ClientCreation.CreateShareServiceClient(options, serviceProvider));
    }

    [Test]
    public void CreateShareServiceClient_ModeServiceProvider_ThrowUnreachableException()
    {
        var options = new FileShareAvailableOptions { Mode = FileClientCreationMode.ServiceProvider };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        _ = Assert.Throws<UnreachableException>(() => ClientCreation.CreateShareServiceClient(options, serviceProvider));
    }
}