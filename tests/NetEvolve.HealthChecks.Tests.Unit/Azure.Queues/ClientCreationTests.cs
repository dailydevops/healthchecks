namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Queues;

using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Queues;

[TestGroup($"{nameof(Azure)}.{nameof(Queues)}")]
public class ClientCreationTests
{
    [Test]
    public void CreateQueueServiceClient_InvalidMode_ThrowUnreachableException()
    {
        var options = new QueueClientAvailableOptions { Mode = (QueueClientCreationMode)13 };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        _ = Assert.Throws<UnreachableException>(() =>
            ClientCreation.CreateQueueServiceClient(options, serviceProvider)
        );
    }

    [Test]
    public void CreateQueueServiceClient_ModeServiceProvider_ThrowUnreachableException()
    {
        var options = new QueueClientAvailableOptions { Mode = QueueClientCreationMode.ServiceProvider };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        _ = Assert.Throws<UnreachableException>(() =>
            ClientCreation.CreateQueueServiceClient(options, serviceProvider)
        );
    }
}
