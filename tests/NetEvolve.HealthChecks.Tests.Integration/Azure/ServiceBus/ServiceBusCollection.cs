namespace NetEvolve.HealthChecks.Tests.Integration.Azure.ServiceBus;

[CollectionDefinition($"{nameof(Azure)}.{nameof(ServiceBus)}")]
public sealed class ServiceBusFixture : ICollectionFixture<ServiceBusContainer>
{
    // This class is used to define a collection fixture for the ServiceBusContainer.
    // It allows sharing the same instance of ServiceBusContainer across multiple tests.
}
