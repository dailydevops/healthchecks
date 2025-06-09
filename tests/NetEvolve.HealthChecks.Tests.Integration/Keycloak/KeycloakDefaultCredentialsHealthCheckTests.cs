namespace NetEvolve.HealthChecks.Tests.Integration.Keycloak;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Tests.Integration.Keycloak.Container;

[ClassDataSource<ContainerDefaultCredentials>(Shared = SharedType.PerTestSession)]
[InheritsTests]
[TestGroup(nameof(Keycloak))]
public sealed class KeycloakDefaultCredentialsHealthCheckTests : KeycloakHealthCheckBaseTests
{
    public KeycloakDefaultCredentialsHealthCheckTests(ContainerDefaultCredentials container)
        : base(container) { }
}
