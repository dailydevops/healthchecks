namespace NetEvolve.HealthChecks.Tests.Integration.Apache.RocketMQ;

using NetEvolve.Extensions.TUnit;

[ClassDataSource<RocketMQContainer>(Shared = SharedType.PerClass)]
[InheritsTests]
[TestGroup($"{nameof(Apache)}.{nameof(RocketMQ)}")]
[TestGroup("Z00TestGroup")]
public sealed class RocketMQHealthCheckTests : RocketMQHealthCheckBaseTests
{
    public RocketMQHealthCheckTests(RocketMQContainer container)
        : base(container) { }
}
