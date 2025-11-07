namespace NetEvolve.HealthChecks.Tests.Unit.OpenSearch;

using System;
using System.Threading.Tasks;
using global::OpenSearch.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.OpenSearch;

[TestGroup(nameof(OpenSearch))]
public sealed class OpenSearchConfigureTests
{
    [Test]
    public void Configure_OnlyOptions_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new OpenSearchConfigure(new ConfigurationBuilder().Build(), services.BuildServiceProvider());
        var options = new OpenSearchOptions();

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
        OpenSearchOptions options
    )
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new OpenSearchConfigure(new ConfigurationBuilder().Build(), services.BuildServiceProvider());

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

    public static IEnumerable<Func<(bool, string?, string?, IList<string>?, OpenSearchOptions)>> GetValidateTestCases()
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
                new OpenSearchOptions { Timeout = -2 }
            );
        yield return () =>
            (
                false,
                "The mode `-1` is not supported.",
                "name",
                null,
                new OpenSearchOptions { Mode = (OpenSearchClientCreationMode)(-1) }
            );

        // Mode: ServiceProvider
        yield return () =>
            (
                false,
                $"No service of type `{nameof(OpenSearchClient)}` registered. Please execute `services.AddSingleton<OpenSearchClient>()`.",
                "name",
                null,
                new OpenSearchOptions { Mode = OpenSearchClientCreationMode.ServiceProvider }
            );

        // Mode: UsernameAndPassword
        yield return () =>
            (
                false,
                "The connection strings list cannot be empty when using the `UsernameAndPassword` client creation mode.",
                "name",
                null,
                new OpenSearchOptions { Mode = OpenSearchClientCreationMode.UsernameAndPassword }
            );
        yield return () =>
            (
                false,
                "The username cannot be null or whitespace when using the `UsernameAndPassword` client creation mode with a password.",
                "name",
                ["connection-string"],
                new OpenSearchOptions { Mode = OpenSearchClientCreationMode.UsernameAndPassword, Password = "password" }
            );
        yield return () =>
            (
                false,
                "The password cannot be null or whitespace when using the `UsernameAndPassword` client creation mode with a username.",
                "name",
                ["connection-string"],
                new OpenSearchOptions { Mode = OpenSearchClientCreationMode.UsernameAndPassword, Username = "username" }
            );
        yield return () =>
            (
                true,
                null,
                "name",
                ["connection-string"],
                new OpenSearchOptions
                {
                    Mode = OpenSearchClientCreationMode.UsernameAndPassword,
                    Username = "username",
                    Password = "password",
                }
            );
    }
}
