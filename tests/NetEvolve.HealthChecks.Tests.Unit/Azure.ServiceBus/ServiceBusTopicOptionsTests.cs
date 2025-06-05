﻿namespace NetEvolve.HealthChecks.Tests.Unit.Azure.ServiceBus;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.ServiceBus;

[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}")]
[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}.Topic")]
public sealed class ServiceBusTopicOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new ServiceBusTopicOptions();
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
