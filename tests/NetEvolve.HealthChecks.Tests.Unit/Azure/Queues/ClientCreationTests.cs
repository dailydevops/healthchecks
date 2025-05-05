namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Queues;

using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Azure.Queues;
using Xunit;

[TestGroup("AzureQueues")]
public class ClientCreationTests
{
    [Fact]
    public void CreateQueueServiceClient_InvalidMode_ThrowUnreachableException()
    {
        var options = new QueueClientAvailableOptions { Mode = (QueueClientCreationMode)13 };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        _ = Assert.Throws<UnreachableException>(() => ClientCreation.CreateQueueServiceClient(options, serviceProvider)
        );
    }

    [Fact]
    public void CreateQueueServiceClient_ModeServiceProvider_ThrowUnreachableException()
    {
        var options = new QueueClientAvailableOptions { Mode = QueueClientCreationMode.ServiceProvider };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        _ = Assert.Throws<UnreachableException>(() => ClientCreation.CreateQueueServiceClient(options, serviceProvider)
        );
    }
}
