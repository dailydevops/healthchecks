namespace NetEvolve.HealthChecks.Tests;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VerifyXunit;

[UsesVerify]
public abstract class HealthCheckTestBase
{
    private const string HealthCheckPath = "/health";

    protected async ValueTask RunAndVerify(Action<IHealthChecksBuilder> healthChecks, Action<IConfigurationBuilder>? config = null, Action<IServiceCollection>? serviceBuilder = null)
    {
        var builder = new WebHostBuilder()
            .ConfigureAppConfiguration((_, configBuilder) =>
            {
                config?.Invoke(configBuilder);
            })
            .ConfigureServices(services =>
            {
                serviceBuilder?.Invoke(services);
                var healthChecksBuilder = services.AddHealthChecks();
                healthChecks?.Invoke(healthChecksBuilder);
            })
            .Configure(app =>
            {
                _ = app.UseHealthChecks(HealthCheckPath, new HealthCheckOptions
                {
                    ResponseWriter = WriteResponse
                });
            });

        using (var server = new TestServer(builder))
        {
            var client = server.CreateClient();
            var response = await client
                .GetAsync(new Uri(HealthCheckPath, UriKind.Relative))
                .ConfigureAwait(false);

            var resultContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var content = string.IsNullOrWhiteSpace(resultContent) ? null : Argon.JToken.Parse(resultContent);

            _ = await Verifier.Verify(content).UseDirectory(GetProjectDirectory());
        }
    }

    private string GetProjectDirectory()
    {
        var directory = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "..");
        var projectName = GetType().Assembly.GetName().Name!;
        var shortName = projectName[(projectName.IndexOf(".", StringComparison.OrdinalIgnoreCase) + 1)..];
        shortName = shortName[..shortName.IndexOf(".Tests.", StringComparison.OrdinalIgnoreCase)];

        var snapshotDirectory = Path.Combine(directory, shortName, "tests", projectName, "_snapshot");
        return Path.GetFullPath(snapshotDirectory);
    }

    private static Task WriteResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var options = new JsonWriterOptions { Indented = true };

        using (var memoryStream = new MemoryStream())
        {
            using (var writer = new Utf8JsonWriter(memoryStream, options))
            {
                writer.WriteStartObject();
                writer.WriteString("status", report.Status.ToString());
                writer.WriteStartArray("results");

                foreach (var (key, value) in report.Entries)
                {
                    writer.WriteStartObject();
                    writer.WriteString("name", key);
                    writer.WriteString("status", value.Status.ToString());
                    writer.WriteString("description", value.Description);
                    WriteException(writer, value.Exception);
                    WriteData(writer, value.Data);
                    WriteTags(writer, value.Tags);
                    writer.WriteEndObject();
                }

                writer.WriteEndArray();
                writer.WriteEndObject();
            }

            return context.Response.WriteAsync(Encoding.UTF8.GetString(memoryStream.ToArray()));
        }
    }

    private static void WriteException(Utf8JsonWriter writer, Exception? exception)
    {
        if (exception is null)
        {
            return;
        }

        writer.WriteStartObject("exception");
        writer.WriteString("message", exception.Message);
        writer.WriteString("type", exception.GetType().FullName);
        if (exception.InnerException is not null)
        {
            writer.WriteStartArray("innerExceptions");
            var inner = exception.InnerException;
            do
            {
                writer.WriteStartObject();
                writer.WriteString("message", inner.Message);
                writer.WriteString("type", inner.GetType().Name);
                writer.WriteEndObject();
                inner = inner.InnerException;
            } while (inner is not null);
            writer.WriteEndArray();
        }
        writer.WriteEndObject();
    }

    private static void WriteData(Utf8JsonWriter writer, IReadOnlyDictionary<string, object> data)
    {
        if (!data.Any())
        {
            return;
        }

        writer.WriteStartObject("data");

        foreach (var item in data)
        {
            writer.WritePropertyName(item.Key);

            JsonSerializer.Serialize(writer, item.Value, item.Value?.GetType() ?? typeof(object));
        }

        writer.WriteEndObject();
    }

    [SuppressMessage("Performance", "CA1851:Possible multiple enumerations of 'IEnumerable' collection", Justification = "As designed.")]
    private static void WriteTags(Utf8JsonWriter writer, IEnumerable<string> tags)
    {
        if (!tags.Any())
        {
            return;
        }

        writer.WriteStartArray("tags");
        foreach (var tag in tags)
        {
            writer.WriteStringValue(tag);
        }
        writer.WriteEndArray();
    }
}
