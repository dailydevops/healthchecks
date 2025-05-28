namespace NetEvolve.HealthChecks.Tests.Integration.Apache.ActiveMq;

public sealed class ActiveMqHealthDefaultCredentialsCheckTests
    : ActiveMqHealthCheckBaseTests,
        IClassFixture<ActiveMqDefaultCredentials>
{
    public ActiveMqHealthDefaultCredentialsCheckTests(ActiveMqDefaultCredentials accessor)
        : base(accessor) { }
}
