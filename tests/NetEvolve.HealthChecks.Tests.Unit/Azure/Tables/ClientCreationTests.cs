namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Tables;

using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Tables;

[TestGroup($"{nameof(Azure)}.{nameof(Tables)}")]
public class ClientCreationTests
{
    [Test]
    public void CreateTableServiceClient_InvalidMode_ThrowUnreachableException()
    {
        var options = new TableClientAvailableOptions { Mode = (TableClientCreationMode)13 };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        _ = Assert.Throws<UnreachableException>(() =>
            ClientCreation.CreateTableServiceClient(options, serviceProvider)
        );
    }
}
