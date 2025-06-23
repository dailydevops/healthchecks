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
        IList<string>? connectionStrings,
        ElasticsearchOptions options
    )
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new ElasticsearchConfigure(new ConfigurationBuilder().Build(), services.BuildServiceProvider());

        if (options is not null)
        {
            foreach (var connectionString in connectionStrings ?? [])
            {
                options.ConnectionStrings.Add(connectionString);
            }
        }

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
        Func<(bool, string?, string?, IList<string>?, ElasticsearchOptions)>
    > GetValidateTestCases()
    {
        yield return () => (false, "The name cannot be null or whitespace.", null, null, null!);
        yield return () => (false, "The name cannot be null or whitespace.", "\t", null, null!);
        yield return () => (false, "The options cannot be null.", "name", null, null!);
        yield return () =>
            (
                false,
                "The timeout value must be a positive number in milliseconds or -1 for an infinite timeout.",
                "name",
                null,
                new ElasticsearchOptions { Timeout = -2 }
            );
        yield return () =>
            (
                false,
                "The mode `-1` is not supported.",
                "name",
                null,
                new ElasticsearchOptions { Mode = (ElasticsearchClientCreationMode)(-1) }
            );

        // Mode: ServiceProvider
        yield return () =>
            (
                false,
                $"No service of type `{nameof(ElasticsearchClient)}` registered. Please execute `services.AddSingleton<ElasticsearchClient>()`.",
                "name",
                null,
                new ElasticsearchOptions { Mode = ElasticsearchClientCreationMode.ServiceProvider }
            );

        // Mode: UsernameAndPassword
        yield return () =>
            (
                false,
                "The connection strings list cannot be empty when using the `UsernameAndPassword` client creation mode.",
                "name",
                null,
                new ElasticsearchOptions { Mode = ElasticsearchClientCreationMode.UsernameAndPassword }
            );
        yield return () =>
            (
                false,
                "The username cannot be null or whitespace when using the `UsernameAndPassword` client creation mode with a password.",
                "name",
                ["connection-string"],
                new ElasticsearchOptions
                {
                    Mode = ElasticsearchClientCreationMode.UsernameAndPassword,
                    Password = "password",
                }
            );
        yield return () =>
            (
                false,
                "The password cannot be null or whitespace when using the `UsernameAndPassword` client creation mode with a username.",
                "name",
                ["connection-string"],
                new ElasticsearchOptions
                {
                    Mode = ElasticsearchClientCreationMode.UsernameAndPassword,
                    Username = "username",
                }
            );
        yield return () =>
            (
                true,
                null,
                "name",
                ["connection-string"],
                new ElasticsearchOptions
                {
                    Mode = ElasticsearchClientCreationMode.UsernameAndPassword,
                    Username = "username",
                    Password = "password",
                }
            );
    }
}
