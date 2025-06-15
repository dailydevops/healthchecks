namespace NetEvolve.HealthChecks.Tests.Unit.Elasticsearch;

using System;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Elasticsearch;

[TestGroup(nameof(Elasticsearch))]
public sealed class ElasticsearchConfigureTests
{
    [Test]
    public void Configue_OnlyOptions_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new ElasticsearchConfigure(new ConfigurationBuilder().Build(), services.BuildServiceProvider());
        var options = new ElasticsearchOptions();

        // Act / Assert
        _ = Assert.Throws<ArgumentException>("name", () => configure.Configure(options));
    }

    [Test]
    [MethodDataSource(nameof(GetValidateTestCases))]
    public async Task Validate_Theory_Expected(
        bool expectedResult,
        string? expectedMessage,
        string? name,
        ElasticsearchOptions options
    )
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new ElasticsearchConfigure(new ConfigurationBuilder().Build(), services.BuildServiceProvider());

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsEqualTo(expectedResult);
            _ = await Assert.That(result.FailureMessage).IsEqualTo(expectedMessage);
        }
    }

    public static IEnumerable<Func<(bool, string?, string?, ElasticsearchOptions)>> GetValidateTestCases()
    {
        yield return () => (false, "The name cannot be null or whitespace.", null, null!);
        yield return () => (false, "The name cannot be null or whitespace.", "\t", null!);
        yield return () => (false, "The options cannot be null.", "name", null!);
        yield return () =>
            (
                false,
                "The timeout cannot be less than infinite (-1).",
                "name",
                new ElasticsearchOptions { Timeout = -2 }
            );
        yield return () =>
            (
                false,
                "The mode `-1` is not supported.",
                "name",
                new ElasticsearchOptions { Mode = (ElasticsearchClientCreationMode)(-1) }
            );

        // Mode: ServiceProvider
        yield return () =>
            (
                false,
                $"No service of type `{nameof(ElasticsearchClient)}` registered. Please execute `services.AddSingleton<ElasticsearchClient>()`.",
                "name",
                new ElasticsearchOptions { Mode = ElasticsearchClientCreationMode.ServiceProvider }
            );

        // Mode: Internal
        yield return () =>
            (
                false,
                "The connection string cannot be null or whitespace when using the `Internal` client creation mode.",
                "name",
                new ElasticsearchOptions { Mode = ElasticsearchClientCreationMode.Internal }
            );
        yield return () =>
            (
                false,
                "The CA certificate fingerprint cannot be null or whitespace when using the `Internal` client creation mode.",
                "name",
                new ElasticsearchOptions
                {
                    Mode = ElasticsearchClientCreationMode.Internal,
                    ConnectionString = "connection-string",
                }
            );
        yield return () =>
            (
                false,
                "The username cannot be null when using the `Internal` client creation mode.",
                "name",
                new ElasticsearchOptions
                {
                    Mode = ElasticsearchClientCreationMode.Internal,
                    ConnectionString = "connection-string",
                }
            );
        yield return () =>
            (
                false,
                "The password cannot be null when using the `Internal` client creation mode.",
                "name",
                new ElasticsearchOptions
                {
                    Mode = ElasticsearchClientCreationMode.Internal,
                    ConnectionString = "connection-string",
                    Username = "username",
                }
            );
        yield return () =>
            (
                true,
                null,
                "name",
                new ElasticsearchOptions
                {
                    Mode = ElasticsearchClientCreationMode.Internal,
                    ConnectionString = "connection-string",
                    Username = "username",
                    Password = "password",
                }
            );
    }
}
