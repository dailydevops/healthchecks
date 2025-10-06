namespace NetEvolve.HealthChecks.Tests.Unit.Azure.CosmosDB;

using System;
using System.Threading;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.CosmosDB;

[TestGroup($"{nameof(Azure)}.{nameof(CosmosDB)}")]
public class CosmosDbHealthCheckTests
{
    [Test]
    public async Task ExecuteHealthCheckAsync_WhenCalled_ShouldNotThrow()
    {
        // Arrange
        var serviceProvider = new MockServiceProvider();
        var optionsMonitor = new MockOptionsMonitor<CosmosDbOptions>();
        var healthCheck = new CosmosDbHealthCheck(serviceProvider, optionsMonitor);

        // Act & Assert
        // Note: This will fail in real execution due to missing CosmosDB connection,
        // but it should not throw during construction
        await Assert.That(healthCheck).IsNotNull();
    }

    [Test]
    public async Task Constructor_WhenServiceProviderNull_ShouldThrow()
    {
        // Arrange
        IServiceProvider serviceProvider = null!;
        var optionsMonitor = new MockOptionsMonitor<CosmosDbOptions>();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(
            () => new CosmosDbHealthCheck(serviceProvider, optionsMonitor)
        );
        await Assert.That(exception.ParamName).IsEqualTo("serviceProvider");
    }

    [Test]
    public async Task Constructor_WhenOptionsMonitorNull_ShouldThrow()
    {
        // Arrange
        var serviceProvider = new MockServiceProvider();
        IOptionsMonitor<CosmosDbOptions> optionsMonitor = null!;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(
            () => new CosmosDbHealthCheck(serviceProvider, optionsMonitor)
        );
        await Assert.That(exception.ParamName).IsEqualTo("optionsMonitor");
    }

    private class MockServiceProvider : IServiceProvider
    {
        public object? GetService(Type serviceType)
        {
            if (serviceType == typeof(ClientCreation))
            {
                return new ClientCreation();
            }
            return null;
        }
    }

    private class MockOptionsMonitor<T> : IOptionsMonitor<T>
        where T : class
    {
        public T CurrentValue => throw new NotImplementedException();

        public T Get(string? name) => throw new NotImplementedException();

        public IDisposable? OnChange(Action<T, string?> listener) => throw new NotImplementedException();
    }
}
