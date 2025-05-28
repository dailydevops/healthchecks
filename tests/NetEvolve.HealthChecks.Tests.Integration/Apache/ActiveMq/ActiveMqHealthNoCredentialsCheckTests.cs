namespace NetEvolve.HealthChecks.Tests.Integration.Apache.ActiveMq;

public sealed class ActiveMqHealthNoCredentialsCheckTests
    : ActiveMqHealthCheckBaseTests,
        IClassFixture<ActiveMqNoCredentials>
{
    public ActiveMqHealthNoCredentialsCheckTests(ActiveMqNoCredentials accessor)
        : base(accessor) { }
}
