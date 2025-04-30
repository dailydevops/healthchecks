namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Blobs;

using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.HealthChecks.Azure.Blobs;
using Xunit;

public class ClientCreationTests
{
    [Fact]
    public void CreateBlobServiceClient_InvalidMode_ThrowUnreachableException()
    {
        var options = new BlobContainerAvailableOptions { Mode = (BlobClientCreationMode)13 };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        _ = Assert.Throws<UnreachableException>(() =>
            ClientCreation.CreateBlobServiceClient(options, serviceProvider)
        );
    }

    [Fact]
    public void CreateBlobServiceClient_ModeServiceProvider_ThrowUnreachableException()
    {
        var options = new BlobContainerAvailableOptions
        {
            Mode = BlobClientCreationMode.ServiceProvider,
        };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        _ = Assert.Throws<UnreachableException>(() =>
            ClientCreation.CreateBlobServiceClient(options, serviceProvider)
        );
    }
}
