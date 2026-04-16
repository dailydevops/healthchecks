namespace NetEvolve.HealthChecks.Tests.Integration.Apache.RocketMQ;

public interface IRocketMQAccessor
{
    string Endpoint { get; }

    string Topic { get; }

    string? AccessKey { get; }

    string? AccessSecret { get; }
}
