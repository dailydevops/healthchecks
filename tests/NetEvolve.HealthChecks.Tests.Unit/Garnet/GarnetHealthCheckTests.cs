namespace NetEvolve.HealthChecks.Tests.Unit.Garnet;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Garnet;

[TestGroup(nameof(Garnet))]
[ExcludeFromCodeCoverage]
public class GarnetHealthCheckTests
{
    [Test]
    public async Task Validate_HealthCheck_IsRegistered()
    {
        // Arrange
        var healthCheck = typeof(GarnetHealthCheck);

        // Act
        var isRegistered = healthCheck.IsAssignableTo(typeof(IHealthCheck));

        // Assert
        _ = await Assert.That(isRegistered).IsTrue();
    }

    [Test]
    public async Task Validate_HealthCheck_IsIDisposable()
    {
        // Arrange
        var healthCheck = typeof(GarnetHealthCheck);

        // Act
        var isDisposable = healthCheck.IsAssignableTo(typeof(IDisposable));

        // Assert
        _ = await Assert.That(isDisposable).IsTrue();
    }
}
