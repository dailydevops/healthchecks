namespace NetEvolve.HealthChecks.Tests.Integration.Keycloak;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Tests.Integration.Keycloak.Container;

[ClassDataSource<ContainerCustomCredentials>(Shared = InstanceSharedType.Keycloak)]
[InheritsTests]
[TestGroup(nameof(Keycloak))]
public sealed class KeycloakCustomCredentialsHealthCheckTests : KeycloakHealthCheckBaseTests
{
    public KeycloakCustomCredentialsHealthCheckTests(ContainerCustomCredentials container)
        : base(container) { }
}
