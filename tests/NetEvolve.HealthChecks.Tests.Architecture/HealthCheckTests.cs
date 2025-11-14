namespace NetEvolve.HealthChecks.Tests.Architecture;

using ArchUnitNET.Domain;
using ArchUnitNET.TUnit;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

[TestGroup(nameof(Architecture))]
[TestGroup(nameof(HealthChecks))]
[TestGroup("Z00TestGroup")]
[TestGroup("Z01TestGroup")]
[TestGroup("Z02TestGroup")]
[TestGroup("Z03TestGroup")]
[TestGroup("Z04TestGroup")]
public class HealthCheckTests
{
    private readonly IObjectProvider<Class> _healthChecks = Classes()
        .That()
        .AreNotAbstract()
        .And()
        .AreAssignableTo(typeof(IHealthCheck));

    [Test]
    public void HealthCheckClass_ShouldBeInternal_Expected()
    {
        var rule = Classes().That().Are(_healthChecks).Should().BeInternal();

        rule.Check(HealthCheckArchitecture.Instance);
    }

    [Test]
    public void HealthCheckClass_ShouldBeSealed_Expected()
    {
        var rule = Classes().That().Are(_healthChecks).Should().BeSealed();

        rule.Check(HealthCheckArchitecture.Instance);
    }

    [Test]
    public void HealthCheckClass_ShouldResideInNamespace_StartsWithNetEvolveExpected()
    {
        var rule = Classes().That().Are(_healthChecks).Should().ResideInNamespaceMatching(@"NetEvolve\.HealthChecks");

        rule.Check(HealthCheckArchitecture.Instance);
    }

    [Test]
    public void HealthCheckClass_ShouldHaveNameEndingWithHealthCheck_Expected()
    {
        var rule = Classes().That().Are(_healthChecks).Should().HaveNameEndingWith("HealthCheck");

        rule.Check(HealthCheckArchitecture.Instance);
    }

    [Test]
    public void HealthCheckConstructors_ShouldBePublic_Expected()
    {
        var rule = MethodMembers()
            .That()
            .AreDeclaredIn(_healthChecks)
            .And()
            .AreConstructors()
            .Should()
            .BePublic()
            .OrShould()
            // Fallback default constructor
            .BePrivate();

        rule.Check(HealthCheckArchitecture.Instance);
    }

    [Test]
    public void HealthCheckMembers_ShouldNotBePublic_Expected()
    {
        var rule = MethodMembers()
            .That()
            .AreDeclaredIn(_healthChecks)
            .And()
            .DoNotHaveNameContaining("CheckHealthAsync")
            .And()
            .AreNoConstructors()
            .Should()
            .NotBePublic();

        rule.Check(HealthCheckArchitecture.Instance);
    }
}
