﻿namespace NetEvolve.HealthChecks.Tests.Unit.Keycloak;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Keycloak;

[TestGroup(nameof(Keycloak))]
public sealed class KeycloakOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new KeycloakOptions();
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
