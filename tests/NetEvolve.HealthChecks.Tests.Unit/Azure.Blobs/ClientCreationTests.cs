namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Blobs;

using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Blobs;

[TestGroup($"{nameof(Azure)}.{nameof(Blobs)}")]
public class ClientCreationTests
{
    [Test]
    public void CreateBlobServiceClient_InvalidMode_ThrowUnreachableException()
    {
        var options = new BlobContainerAvailableOptions { Mode = (BlobClientCreationMode)13 };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        _ = Assert.Throws<UnreachableException>(() => ClientCreation.CreateBlobServiceClient(options, serviceProvider));
    }

    [Test]
    public void CreateBlobServiceClient_ModeServiceProvider_ThrowUnreachableException()
    {
        var options = new BlobContainerAvailableOptions { Mode = BlobClientCreationMode.ServiceProvider };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        _ = Assert.Throws<UnreachableException>(() => ClientCreation.CreateBlobServiceClient(options, serviceProvider));
    }
}
