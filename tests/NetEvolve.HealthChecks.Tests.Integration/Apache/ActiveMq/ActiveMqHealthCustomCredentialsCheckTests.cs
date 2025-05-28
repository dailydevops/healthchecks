namespace NetEvolve.HealthChecks.Tests.Integration.Apache.ActiveMq;

public sealed class ActiveMqHealthCustomCredentialsCheckTests
    : ActiveMqHealthCheckBaseTests,
        IClassFixture<ActiveMqCustomCredentials>
{
    public ActiveMqHealthCustomCredentialsCheckTests(ActiveMqCustomCredentials accessor)
        : base(accessor) { }
}
