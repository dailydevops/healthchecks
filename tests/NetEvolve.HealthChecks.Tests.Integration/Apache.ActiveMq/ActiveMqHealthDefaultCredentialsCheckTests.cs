namespace NetEvolve.HealthChecks.Tests.Integration.Apache.ActiveMq;

using NetEvolve.Extensions.TUnit;

[ClassDataSource<ActiveMqDefaultCredentials>(Shared = InstanceSharedType.ActiveMQ)]
[InheritsTests]
[TestGroup($"{nameof(Apache)}.{nameof(ActiveMq)}")]
public sealed class ActiveMqHealthDefaultCredentialsCheckTests : ActiveMqHealthCheckBaseTests
{
    public ActiveMqHealthDefaultCredentialsCheckTests(ActiveMqDefaultCredentials accessor)
        : base(accessor) { }
}
