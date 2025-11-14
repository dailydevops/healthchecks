namespace NetEvolve.HealthChecks.Tests.Integration.Apache.ActiveMq;

using NetEvolve.Extensions.TUnit;

[ClassDataSource<ActiveMqCustomCredentials>(Shared = InstanceSharedType.ActiveMQ)]
[InheritsTests]
[TestGroup($"{nameof(Apache)}.{nameof(ActiveMq)}")]
[TestGroup("Z00TestGroup")]
public sealed class ActiveMqHealthCustomCredentialsCheckTests : ActiveMqHealthCheckBaseTests
{
    public ActiveMqHealthCustomCredentialsCheckTests(ActiveMqCustomCredentials accessor)
        : base(accessor) { }
}
