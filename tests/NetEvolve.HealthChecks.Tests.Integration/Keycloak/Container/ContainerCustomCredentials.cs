namespace NetEvolve.HealthChecks.Tests.Integration.Keycloak.Container;

using System;

public sealed class ContainerCustomCredentials : ContainerBase
{
    public ContainerCustomCredentials()
        : base($"{Guid.NewGuid:D}", $"{Guid.NewGuid:D}") { }
}
