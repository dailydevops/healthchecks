namespace NetEvolve.HealthChecks.Http;

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

internal sealed class HttpConfigure : IConfigureNamedOptions<HttpOptions>
{
    private readonly IConfiguration _configuration;

    public HttpConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, HttpOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:Http:{name}", options);
    }

    public void Configure(HttpOptions options) => Configure(Options.DefaultName, options);
}
