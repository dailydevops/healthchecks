namespace NetEvolve.HealthChecks.Tests.Integration.Apache.ActiveMq;

public interface IActiveMQAccessor
{
    string BrokerAddress { get; }

    string? Username { get; }

    string? Password { get; }
}
