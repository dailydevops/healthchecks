namespace NetEvolve.HealthChecks.Tests.Integration.Azure.IotHub;

using System;
using System.Net.Http;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging.Abstractions;

/// <summary>
/// Provides a mock IoT Hub container for integration testing using WireMock.
/// This simulates the Azure IoT Hub REST API endpoints needed for health checking.
/// </summary>
public sealed class IoTHubMockContainer : IAsyncInitializer, IAsyncDisposable
{
    public const string TestHubName = "test-hub";
    private const int WireMockPort = 8080;

    private readonly IContainer _container = new ContainerBuilder()
        .WithImage("wiremock/wiremock:latest")
        .WithPortBinding(WireMockPort, true)
        .WithCommand("--port 8080", "--verbose")
        .WithLogger(NullLogger.Instance)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(WireMockPort))
        .Build();

    /// <summary>
    /// Gets the base URL for the mock IoT Hub service.
    /// </summary>
    public Uri BaseUrl => new Uri($"http://{_container.Hostname}:{_container.GetMappedPublicPort(WireMockPort)}");

    /// <summary>
    /// Gets a mock connection string for the IoT Hub (note: this won't work with real Azure SDK due to hostname validation).
    /// </summary>
    public string MockConnectionString =>
        $"HostName={_container.Hostname}:{_container.GetMappedPublicPort(WireMockPort)};SharedAccessKeyName=iothubowner;SharedAccessKey=dGVzdGtleQ==";

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync()
    {
        await _container.StartAsync().ConfigureAwait(false);
        await SetupMockEndpoints().ConfigureAwait(false);
    }

    /// <summary>
    /// Sets up mock endpoints that simulate Azure IoT Hub REST API responses.
    /// </summary>
    private async Task SetupMockEndpoints()
    {
        using var httpClient = new HttpClient();
        var adminUrl = new Uri(BaseUrl, "/__admin/mappings");

        // Mock the service statistics endpoint
        var serviceStatsMock = """
            {
              "request": {
                "method": "GET",
                "url": "/statistics/service"
              },
              "response": {
                "status": 200,
                "headers": {
                  "Content-Type": "application/json"
                },
                "body": "{\"deviceCount\": 5, \"enabledDeviceCount\": 3, \"disabledDeviceCount\": 2}"
              }
            }
            """;

        // Mock authentication endpoint (for cases where auth is checked)
        var authMock = """
            {
              "request": {
                "method": "POST",
                "url": "/devices/query"
              },
              "response": {
                "status": 200,
                "headers": {
                  "Content-Type": "application/json"
                },
                "body": "[]"
              }
            }
            """;

        // Add the mocks
        using var serviceContent = new StringContent(serviceStatsMock, System.Text.Encoding.UTF8, "application/json");
        using var authContent = new StringContent(authMock, System.Text.Encoding.UTF8, "application/json");

        _ = await httpClient.PostAsync(adminUrl, serviceContent).ConfigureAwait(false);
        _ = await httpClient.PostAsync(adminUrl, authContent).ConfigureAwait(false);
    }
}
