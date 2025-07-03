namespace NetEvolve.HealthChecks.Tests.Unit.LiteDB;

using System;
using System.Threading;
using System.Threading.Tasks;
using global::LiteDB;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.LiteDB;
using NSubstitute;

[TestGroup(nameof(LiteDB))]
public sealed class LiteDBHealthCheckTests
{
    [Test]
    public async Task CheckHealthAsync_WhenContextNull_ThrowArgumentNullException()
    {
        // Arrange
        var optionsMonitor = Substitute.For<IOptionsMonitor<LiteDBOptions>>();
        var check = new LiteDBHealthCheck(optionsMonitor);

        // Act
        async Task Act() => _ = await check.CheckHealthAsync(null!, default);

        // Assert
        _ = await Assert.ThrowsAsync<ArgumentNullException>("context", Act);
    }
}
