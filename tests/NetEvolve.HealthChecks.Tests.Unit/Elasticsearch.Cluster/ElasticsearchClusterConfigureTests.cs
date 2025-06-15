namespace NetEvolve.HealthChecks.Tests.Unit.Elasticsearch.Cluster;

using System;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Elasticsearch.Cluster;

[TestGroup($"{nameof(Elasticsearch)}.{nameof(Cluster)}")]
public sealed class ElasticsearchClusterConfigureTests
{
    [Test]
    public void Configue_OnlyOptions_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new ElasticsearchClusterConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );
        var options = new ElasticsearchClusterOptions();

        // Act / Assert
        _ = Assert.Throws<ArgumentException>("name", () => configure.Configure(options));
    }

    [Test]
    [MethodDataSource(nameof(GetValidateTestCases))]
    public async Task Validate_Theory_Expected(
        bool expectedResult,
        string? expectedMessage,
        string? name,
        ElasticsearchClusterOptions options
    )
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new ElasticsearchClusterConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsEqualTo(expectedResult);
            _ = await Assert.That(result.FailureMessage).IsEqualTo(expectedMessage);
        }
    }

    public static IEnumerable<Func<(bool, string?, string?, ElasticsearchClusterOptions)>> GetValidateTestCases()
    {
        yield return () => (false, "The name cannot be null or whitespace.", null, null!);
        yield return () => (false, "The name cannot be null or whitespace.", "\t", null!);
        yield return () => (false, "The options cannot be null.", "name", null!);
        yield return () =>
            (
                false,
                "The timeout cannot be less than infinite (-1).",
                "name",
                new ElasticsearchClusterOptions { Timeout = -2 }
            );
        yield return () =>
            (
                false,
                "The mode `-1` is not supported.",
                "name",
                new ElasticsearchClusterOptions { Mode = (ElasticsearchClusterClientCreationMode)(-1) }
            );

        // Mode: ServiceProvider
        yield return () =>
            (
                false,
                $"No service of type `{nameof(ElasticsearchClient)}` registered. Please execute `services.AddSingleton<ElasticsearchClient>()`.",
                "name",
                new ElasticsearchClusterOptions { Mode = ElasticsearchClusterClientCreationMode.ServiceProvider }
            );

        // Mode: Internal
        yield return () =>
            (
                false,
                "The connection strings list cannot be null or empty when using the `Internal` client creation mode.",
                "name",
                new ElasticsearchClusterOptions { Mode = ElasticsearchClusterClientCreationMode.Internal }
            );
        yield return () =>
            (
                false,
                "The username cannot be null or whitespace when using the `Internal` client creation mode with a password.",
                "name",
                new ElasticsearchClusterOptions
                {
                    Mode = ElasticsearchClusterClientCreationMode.Internal,
                    ConnectionStrings = ["connection-string"],
                    Password = "password",
                }
            );
        yield return () =>
            (
                false,
                "The password cannot be null or whitespace when using the `Internal` client creation mode with a username.",
                "name",
                new ElasticsearchClusterOptions
                {
                    Mode = ElasticsearchClusterClientCreationMode.Internal,
                    ConnectionStrings = ["connection-string"],
                    Username = "username",
                }
            );
        yield return () =>
            (
                true,
                null,
                "name",
                new ElasticsearchClusterOptions
                {
                    Mode = ElasticsearchClusterClientCreationMode.Internal,
                    ConnectionStrings = ["connection-string"],
                    Username = "username",
                    Password = "password",
                }
            );
    }
}
