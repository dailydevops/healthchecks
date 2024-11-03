namespace NetEvolve.HealthChecks.Azure.Blobs.Tests.Unit;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.XUnit;
using Xunit;

[ExcludeFromCodeCoverage]
[UnitTest]
public class ClientCreationTests
{
    [Fact]
    public void CreateBlobServiceClient_InvalidMode_ThrowUnreachableException()
    {
        var options = new BlobContainerAvailableOptions { Mode = (BlobClientCreationMode)13 };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        _ = Assert.Throws<UnreachableException>(
            () => ClientCreation.CreateBlobServiceClient(options, serviceProvider)
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

        _ = Assert.Throws<UnreachableException>(
            () => ClientCreation.CreateBlobServiceClient(options, serviceProvider)
        );
    }
}
