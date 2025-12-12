namespace NetEvolve.HealthChecks.Tests.Unit.Meilisearch;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Meilisearch;

[TestGroup(nameof(Meilisearch))]
[ExcludeFromCodeCoverage]
public class MeilisearchHealthCheckTests
{
    [Test]
    public async Task Validate_HealthCheck_IsRegistered()
    {
        // Arrange
        var healthCheck = typeof(MeilisearchHealthCheck);

        // Act
        var isRegistered = healthCheck.IsAssignableTo(typeof(IHealthCheck));

        // Assert
        _ = await Assert.That(isRegistered).IsTrue();
    }

    [Test]
    public async Task Validate_HealthCheck_IsIDisposable()
    {
        // Arrange
        var healthCheck = typeof(MeilisearchHealthCheck);

        // Act
        var isDisposable = healthCheck.IsAssignableTo(typeof(IDisposable));

        // Assert
        _ = await Assert.That(isDisposable).IsTrue();
    }
}
