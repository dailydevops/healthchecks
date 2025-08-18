namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Synapse;

using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Synapse;

[TestGroup($"{nameof(Azure)}.{nameof(Synapse)}")]
public class ClientCreationTests
{
    [Test]
    public void CreateArtifactsClient_InvalidMode_ThrowUnreachableException()
    {
        var options = new SynapseWorkspaceAvailableOptions { Mode = (SynapseClientCreationMode)13 };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        _ = Assert.Throws<UnreachableException>(() => ClientCreation.CreateArtifactsClient(options, serviceProvider));
    }

    [Test]
    public void CreateArtifactsClient_ModeServiceProvider_ThrowUnreachableException()
    {
        var options = new SynapseWorkspaceAvailableOptions { Mode = SynapseClientCreationMode.ServiceProvider };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        _ = Assert.Throws<UnreachableException>(() => ClientCreation.CreateArtifactsClient(options, serviceProvider));
    }
}