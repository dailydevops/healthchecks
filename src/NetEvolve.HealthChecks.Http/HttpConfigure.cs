namespace NetEvolve.HealthChecks.Http;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

internal sealed class HttpConfigure : IConfigureNamedOptions<HttpOptions>
{
    private readonly IConfiguration _configuration;

    public HttpConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, HttpOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        var section = _configuration.GetSection($"HealthChecks:Http:{name}");
        if (!section.Exists())
        {
            return;
        }

        _ = section.Bind(options);
    }

    public void Configure(HttpOptions options) => Configure(Options.DefaultName, options);
}
