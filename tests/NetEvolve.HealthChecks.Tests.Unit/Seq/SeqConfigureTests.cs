namespace NetEvolve.HealthChecks.Tests.Unit.Seq;

using System;
using global::Seq.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Seq;

[TestGroup(nameof(Seq))]
public sealed class SeqConfigureTests
{
    [Test]
    public void Configure_OnlyOptions_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new SeqConfigure(new ConfigurationBuilder().Build(), services.BuildServiceProvider());
        var options = new NetEvolve.HealthChecks.Seq.SeqOptions();

        // Act / Assert
        _ = Assert.Throws<ArgumentException>("name", () => configure.Configure(options));
    }

    [Test]
    [MethodDataSource(nameof(GetValidateTestCases))]
    public async Task Validate_Theory_Expected(
        bool expectedResult,
        string? expectedMessage,
        string? name,
        NetEvolve.HealthChecks.Seq.SeqOptions options
    )
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new SeqConfigure(new ConfigurationBuilder().Build(), services.BuildServiceProvider());

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsEqualTo(expectedResult);
            _ = await Assert.That(result.FailureMessage).IsEqualTo(expectedMessage);
        }
    }

    public static IEnumerable<
        Func<(bool, string?, string?, NetEvolve.HealthChecks.Seq.SeqOptions)>
    > GetValidateTestCases()
    {
        yield return () => (false, "The name cannot be null or whitespace.", null, null!);
        yield return () => (false, "The name cannot be null or whitespace.", "\t", null!);
        yield return () => (false, "The options cannot be null.", "name", null!);
        yield return () =>
            (
                false,
                "The timeout value must be a positive number in milliseconds or -1 for an infinite timeout.",
                "name",
                new NetEvolve.HealthChecks.Seq.SeqOptions { Timeout = -2 }
            );
        yield return () =>
            (
                false,
                "The mode `-1` is not supported.",
                "name",
                new NetEvolve.HealthChecks.Seq.SeqOptions { Mode = (SeqClientCreationMode)(-1) }
            );

        // Mode: ServiceProvider
        yield return () =>
            (
                false,
                $"No service of type `{nameof(SeqConnection)}` registered. Please execute `services.AddSingleton(<client instance>)`.",
                "name",
                new NetEvolve.HealthChecks.Seq.SeqOptions { Mode = SeqClientCreationMode.ServiceProvider }
            );

        // Mode: ServerUrl
        yield return () =>
            (
                false,
                "The server url cannot be null when using the `ServerUrl` client creation mode.",
                "name",
                new NetEvolve.HealthChecks.Seq.SeqOptions { Mode = SeqClientCreationMode.ServerUrl }
            );
        yield return () =>
            (
                false,
                "The server url must be an absolute url when using the `ServerUrl` client creation mode.",
                "name",
                new NetEvolve.HealthChecks.Seq.SeqOptions
                {
                    Mode = SeqClientCreationMode.ServerUrl,
                    ServerUrl = new Uri("relative/path", UriKind.Relative),
                }
            );
        yield return () =>
            (
                true,
                null,
                "name",
                new NetEvolve.HealthChecks.Seq.SeqOptions
                {
                    Mode = SeqClientCreationMode.ServerUrl,
                    ServerUrl = new Uri("http://localhost:5341"),
                }
            );
    }
}
