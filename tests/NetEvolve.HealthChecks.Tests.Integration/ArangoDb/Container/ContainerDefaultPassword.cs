namespace NetEvolve.HealthChecks.Tests.Integration.ArangoDb.Container;

using Testcontainers.ArangoDb;

public sealed class ContainerDefaultPassword : ContainerBase
{
    public ContainerDefaultPassword()
        : base(ArangoDbBuilder.DefaultPassword) { }
}
