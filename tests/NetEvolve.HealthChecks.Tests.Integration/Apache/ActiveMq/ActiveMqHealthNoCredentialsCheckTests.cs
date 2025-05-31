namespace NetEvolve.HealthChecks.Tests.Integration.Apache.ActiveMq;

using NetEvolve.Extensions.TUnit;

[ClassDataSource<ActiveMqNoCredentials>(Shared = SharedType.PerTestSession)]
[InheritsTests]
[TestGroup($"{nameof(Apache)}.{nameof(ActiveMq)}")]
public sealed class ActiveMqHealthNoCredentialsCheckTests : ActiveMqHealthCheckBaseTests
{
    public ActiveMqHealthNoCredentialsCheckTests(ActiveMqNoCredentials accessor)
        : base(accessor) { }
}
