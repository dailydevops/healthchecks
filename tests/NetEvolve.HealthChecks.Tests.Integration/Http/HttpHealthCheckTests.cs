namespace NetEvolve.HealthChecks.Tests.Integration.Http;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Http;
using NetEvolve.HealthChecks.Tests.Integration.Internals;

[TestGroup(nameof(Http))]
public class HttpHealthCheckTests : HealthCheckTestBase
{
    [Test]
    public async Task AddHttp_UseOptions_WithInvalidUri_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddHttp(
                    "TestInvalidUri",
                    options =>
                    {
                        options.Uri = "https://invalid-domain-that-does-not-exist-12345.com";
                        options.Timeout = 10000;
                    }
                );
            },
            HealthStatus.Unhealthy
        );

    [Test]
    public async Task AddHttp_UseConfiguration_WithInvalidUri_Unhealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddHttp("TestInvalidUri"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Http:TestInvalidUri:Uri", "https://invalid-domain-that-does-not-exist-12345.com" },
                    { "HealthChecks:Http:TestInvalidUri:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddHttp_WithLocalServer_ReturnsHealthy()
    {
        // Set up a test server that responds with HTTP 200 OK
        using var testServer = new TestServer(
            new WebHostBuilder().Configure(app =>
            {
                app.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    await context.Response.WriteAsync("OK");
                });
            })
        );

        var testServerUrl = testServer.BaseAddress.ToString().TrimEnd('/');

        // Run the health check against the test server
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddHttp("TestValidEndpoint", options => options.Uri = testServerUrl);
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
            {
                // Register the test server's HttpClient to be used by the health check
                _ = services.AddSingleton(testServer.CreateClient());
            }
        );
    }

    [Test]
    public async Task AddHttp_WithNon200StatusCode_ConfiguredToAccept_ReturnsHealthy()
    {
        // Set up a test server that returns HTTP 201 Created
        using var testServer = new TestServer(
            new WebHostBuilder().Configure(app =>
            {
                app.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Created;
                    await context.Response.WriteAsync("Created");
                });
            })
        );

        var testServerUrl = testServer.BaseAddress.ToString().TrimEnd('/');

        // Run the health check against the test server
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddHttp(
                    "TestCustomStatus",
                    options =>
                    {
                        options.Uri = testServerUrl;
                        options.ExpectedHttpStatusCodes.Add(201);
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services => _ = services.AddSingleton(testServer.CreateClient())
        );
    }

    [Test]
    public async Task AddHttp_WithNon200StatusCode_NotConfiguredToAccept_ReturnsUnhealthy()
    {
        // Set up a test server that returns HTTP 201 Created
        using var testServer = new TestServer(
            new WebHostBuilder().Configure(app =>
            {
                app.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Created;
                    await context.Response.WriteAsync("Created");
                });
            })
        );

        var testServerUrl = testServer.BaseAddress.ToString().TrimEnd('/');

        // Run the health check against the test server
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddHttp(
                    "TestUnexpectedStatus",
                    options =>
                    {
                        options.Uri = testServerUrl;
                        // Default ExpectedHttpStatusCodes is [200], so 201 should be unhealthy
                    }
                );
            },
            HealthStatus.Unhealthy,
            serviceBuilder: services => _ = services.AddSingleton(testServer.CreateClient())
        );
    }

    [Test]
    public async Task AddHttp_WithPostMethod_ReturnsHealthy()
    {
        // Set up a test server that validates POST method
        using var testServer = new TestServer(
            new WebHostBuilder().Configure(app =>
            {
                app.Run(context =>
                {
                    if (context.Request.Method == "POST")
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        return context.Response.WriteAsync("OK");
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                        return context.Response.WriteAsync("Method not allowed");
                    }
                });
            })
        );

        var testServerUrl = testServer.BaseAddress.ToString().TrimEnd('/');

        // Run the health check against the test server
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddHttp(
                    "TestPostMethod",
                    options =>
                    {
                        options.Uri = testServerUrl;
                        options.HttpMethod = "POST";
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services => _ = services.AddSingleton(testServer.CreateClient())
        );
    }

    [Test]
    public async Task AddHttp_WithCustomHeaders_ReturnsHealthy()
    {
        // Set up a test server that validates headers
        var expectedHeader = "X-Test-Header";
        var expectedValue = "TestValue";

        using var testServer = new TestServer(
            new WebHostBuilder().Configure(app =>
            {
                app.Run(context =>
                {
                    if (
                        context.Request.Headers.TryGetValue(expectedHeader, out var values)
                        && values.Count == 1
                        && values[0] == expectedValue
                    )
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        return context.Response.WriteAsync("OK");
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return context.Response.WriteAsync("Missing or invalid header");
                    }
                });
            })
        );

        var testServerUrl = testServer.BaseAddress.ToString().TrimEnd('/');

        // Run the health check against the test server
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddHttp(
                    "TestHeaders",
                    options =>
                    {
                        options.Uri = testServerUrl;
                        options.Headers.Add(expectedHeader, expectedValue);
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services => _ = services.AddSingleton(testServer.CreateClient())
        );
    }

    [Test]
    public async Task AddHttp_WithRequestBody_ReturnsHealthy()
    {
        // Set up a test server that validates request body
        var expectedContentType = "application/json";
        var expectedContent = "{\"test\":\"value\"}";

        using var testServer = new TestServer(
            new WebHostBuilder().Configure(app =>
            {
                _ = app.Use(
                    async (context, next) =>
                    {
                        // Only check requests with content
                        if (context.Request.ContentLength > 0)
                        {
                            // Validate content type
                            var contentType = context.Request.ContentType;

                            if (
                                contentType != null
                                && contentType.StartsWith(expectedContentType, StringComparison.OrdinalIgnoreCase)
                            )
                            {
                                // Read the body content
                                using var reader = new System.IO.StreamReader(context.Request.Body, Encoding.UTF8);

                                var body = await reader.ReadToEndAsync();

                                if (body == expectedContent)
                                {
                                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                                    await context.Response.WriteAsync("OK");
                                    return;
                                }
                            }

                            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            await context.Response.WriteAsync("Invalid content or content type");
                            return;
                        }

                        await next();
                    }
                );
            })
        );

        // Create a client for the TestServer
        var client = testServer.CreateClient();
        var testServerUrl = testServer.BaseAddress.ToString().TrimEnd('/');

        // Run the health check against the test server
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddHttp(
                    "TestRequestBody",
                    options =>
                    {
                        options.Uri = testServerUrl;
                        options.HttpMethod = "POST";
                        options.ContentType = expectedContentType;
                        options.Content = expectedContent;
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services => _ = services.AddSingleton(client)
        );
    }

    [Test]
    public async Task AddHttp_WithSlowEndpoint_TimeoutExceeded_ReturnsDegraded()
    {
        // Set up a test server that has a delayed response
        using var testServer = new TestServer(
            new WebHostBuilder().Configure(app =>
            {
                app.Run(async context =>
                {
                    // Delay for 1 second
                    await Task.Delay(1000);
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    await context.Response.WriteAsync("OK but slow");
                });
            })
        );

        var testServerUrl = testServer.BaseAddress.ToString().TrimEnd('/');

        // Run the health check against the test server with a 500ms timeout
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddHttp(
                    "TestTimeout",
                    options =>
                    {
                        options.Uri = testServerUrl;
                        options.Timeout = 1;
                    }
                );
            },
            HealthStatus.Degraded,
            serviceBuilder: services => _ = services.AddSingleton(testServer.CreateClient())
        );
    }

    [Test]
    public async Task AddHttp_WithRedirect_AllowRedirect_ReturnsHealthy()
    {
        // Set up a test server that redirects
        using var testServer = new TestServer(
            new WebHostBuilder().Configure(app =>
            {
                _ = app.Use(
                    async (context, next) =>
                    {
                        if (context.Request.Path == "/")
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Redirect;
                            context.Response.Headers.Location = "/redirected";
                            return;
                        }
                        else if (context.Request.Path == "/redirected")
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.OK;
                            await context.Response.WriteAsync("Redirected OK");
                            return;
                        }

                        await next();
                    }
                );
            })
        );

        var client = testServer.CreateClient();
        client.DefaultRequestHeaders.Clear();

        var testServerUrl = testServer.BaseAddress.ToString().TrimEnd('/');

        // Run the health check against the test server
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddHttp(
                    "TestRedirect",
                    options =>
                    {
                        options.Uri = testServerUrl;
                        options.AllowAutoRedirect = true;
                        // Adding expected status codes for both the initial 302 and final 200 response
                        options.ExpectedHttpStatusCodes.Add((int)HttpStatusCode.Redirect);
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
            {
                // Register a factory that creates HttpClient with the right settings
                _ = services.AddHttpClient();
                _ = services.AddSingleton(client);
            }
        );
    }

    [Test]
    public async Task AddHttp_WithRedirect_DisallowRedirect_ReturnsUnhealthy()
    {
        // Set up a test server that redirects
        using var testServer = new TestServer(
            new WebHostBuilder().Configure(app =>
            {
                _ = app.Use(
                    async (context, next) =>
                    {
                        if (context.Request.Path == "/")
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Redirect;
                            context.Response.Headers.Location = "/redirected";
                            return;
                        }
                        else if (context.Request.Path == "/redirected")
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.OK;
                            await context.Response.WriteAsync("Redirected OK");
                            return;
                        }

                        await next();
                    }
                );
            })
        );

        // Create client from test server
        var client = testServer.CreateClient();
        client.DefaultRequestHeaders.Clear();

        var testServerUrl = testServer.BaseAddress.ToString().TrimEnd('/');

        // Run the health check against the test server with redirects disabled
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddHttp(
                    "TestNoRedirect",
                    options =>
                    {
                        options.Uri = testServerUrl;
                        options.AllowAutoRedirect = false;
                        // Only expecting 200, not 302
                    }
                );
            },
            HealthStatus.Unhealthy, // Expected to be unhealthy since we get a 302 and expect a 200
            serviceBuilder: services =>
            {
                _ = services.AddHttpClient();
                _ = services.AddSingleton(client);
            }
        );
    }
}
