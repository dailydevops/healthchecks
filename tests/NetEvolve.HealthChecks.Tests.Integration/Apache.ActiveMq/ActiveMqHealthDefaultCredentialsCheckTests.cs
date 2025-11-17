namespace NetEvolve.HealthChecks.Tests.Integration.Apache.ActiveMq;

using NetEvolve.Extensions.TUnit;

[ClassDataSource<ActiveMqDefaultCredentials>(Shared = SharedType.PerClass)]
[InheritsTests]
[TestGroup($"{nameof(Apache)}.{nameof(ActiveMq)}")]
[TestGroup("Z00TestGroup")]
public sealed class ActiveMqHealthDefaultCredentialsCheckTests : ActiveMqHealthCheckBaseTests
{
    public ActiveMqHealthDefaultCredentialsCheckTests(ActiveMqDefaultCredentials accessor)
        : base(accessor) { }
}
