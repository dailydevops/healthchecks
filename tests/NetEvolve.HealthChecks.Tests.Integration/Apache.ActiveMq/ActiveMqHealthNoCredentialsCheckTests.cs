namespace NetEvolve.HealthChecks.Tests.Integration.Apache.ActiveMq;

using NetEvolve.Extensions.TUnit;

[ClassDataSource<ActiveMqNoCredentials>(Shared = SharedType.PerClass)]
[InheritsTests]
[TestGroup($"{nameof(Apache)}.{nameof(ActiveMq)}")]
[TestGroup("Z00TestGroup")]
public sealed class ActiveMqHealthNoCredentialsCheckTests : ActiveMqHealthCheckBaseTests
{
    public ActiveMqHealthNoCredentialsCheckTests(ActiveMqNoCredentials accessor)
        : base(accessor) { }
}
