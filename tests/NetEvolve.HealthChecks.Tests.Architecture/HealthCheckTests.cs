namespace NetEvolve.HealthChecks.Tests.Architecture;

using ArchUnitNET.Domain;
using ArchUnitNET.xUnit;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.XUnit;
using Xunit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

[TestGroup(nameof(HealthChecks))]
public class HealthCheckTests
{
    private readonly IObjectProvider<Class> _healthChecks = Classes()
        .That()
        .AreNotAbstract()
        .And()
        .AreAssignableTo(typeof(IHealthCheck));

    [Fact]
    public void Class_ShouldBeInternal_Expected()
    {
        var rule = Classes().That().Are(_healthChecks).Should().BeInternal();

        rule.Check(HealthCheckArchitecture.Instance);
    }

    [Fact]
    public void Class_ShouldBeSealed_Expected()
    {
        var rule = Classes().That().Are(_healthChecks).Should().BeSealed();

        rule.Check(HealthCheckArchitecture.Instance);
    }

    [Fact]
    public void Class_ShouldResideInNamespace_StartsWithNetEvolveExpected()
    {
        var rule = Classes().That().Are(_healthChecks).Should().ResideInNamespace(@"NetEvolve\.HealthChecks", true);

        rule.Check(HealthCheckArchitecture.Instance);
    }

    [Fact]
    public void Class_ShouldEndsWithHealthCheck_Expected()
    {
        var rule = Classes().That().Are(_healthChecks).Should().HaveNameEndingWith("HealthCheck");

        rule.Check(HealthCheckArchitecture.Instance);
    }

    [Fact]
    public void Constructors_ShouldBePublic_Expected()
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

    [Fact]
    public void Members_ShouldNotBePublic_Expected()
    {
        var healthCheckMembers = MethodMembers().That().AreDeclaredIn(typeof(IHealthCheck));

        var rule = MethodMembers()
            .That()
            .AreDeclaredIn(_healthChecks)
            .And()
            .AreNot(healthCheckMembers)
            .And()
            .AreNoConstructors()
            .Should()
            .NotBePublic();

        rule.Check(HealthCheckArchitecture.Instance);
    }
}
