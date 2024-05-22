namespace NetEvolve.HealthChecks.Azure.Tables.Tests.Unit;

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
    public void CreateTableServiceClient_InvalidMode_ThrowUnreachableException()
    {
        var options = new TableClientAvailableOptions { Mode = (TableClientCreationMode)13 };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        _ = Assert.Throws<UnreachableException>(
            () => ClientCreation.CreateTableServiceClient(options, serviceProvider)
        );
    }
}
