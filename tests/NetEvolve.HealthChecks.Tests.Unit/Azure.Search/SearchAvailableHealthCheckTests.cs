namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Search;

using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using global::Azure;
using global::Azure.Core.Pipeline;
using global::Azure.Search.Documents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Search;

[TestGroup($"{nameof(Azure)}.{nameof(Search)}")]
public sealed class SearchAvailableHealthCheckTests
{
    private const string TestName = $"{nameof(Azure)}.{nameof(Search)}";

    [Test]
    public async Task CheckHealthAsync_WhenIndexReturnsDocumentCount_ShouldReturnHealthy(
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Arrange
        using var handler = new StubHandler(HttpStatusCode.OK, "42");
        using var httpClient = new HttpClient(handler);
        var clientOptions = new SearchClientOptions { Transport = new HttpClientTransport(httpClient) };
        clientOptions.Retry.MaxRetries = 0;

        var services = new ServiceCollection();
        _ = services.AddSingleton(
            new SearchClient(
                new Uri("https://unit-test.search.windows.net"),
                "test-index",
                new AzureKeyCredential("unit-test-key"),
                clientOptions
            )
        );
        _ = services.Configure<SearchAvailableOptions>(
            TestName,
            options =>
            {
                options.IndexName = "test-index";
                options.Mode = ClientCreationMode.ServiceProvider;
                options.Timeout = 10_000;
            }
        );
        await using var serviceProvider = services.BuildServiceProvider();

        var check = new SearchAvailableHealthCheck(
            serviceProvider,
            serviceProvider.GetRequiredService<IOptionsMonitor<SearchAvailableOptions>>()
        );
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(TestName, check, null, null),
        };

        // Act
        var result = await check.CheckHealthAsync(context, cancellationToken);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Healthy);
            _ = await Assert.That(handler.CallCount).IsEqualTo(1);
        }
    }

    private sealed class StubHandler(HttpStatusCode statusCode, string content) : HttpMessageHandler
    {
        public int CallCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            CallCount++;
            return Task.FromResult(
                new HttpResponseMessage(statusCode)
                {
                    Content = new StringContent(content, Encoding.UTF8, "application/json"),
                }
            );
        }
    }
}
