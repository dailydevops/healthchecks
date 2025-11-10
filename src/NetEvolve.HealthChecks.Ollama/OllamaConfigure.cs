namespace NetEvolve.HealthChecks.Ollama;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class OllamaConfigure : IConfigureNamedOptions<OllamaOptions>, IValidateOptions<OllamaOptions>
{
    private readonly IConfiguration _configuration;

    public OllamaConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, OllamaOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:Ollama:{name}", options);
    }

    public void Configure(OllamaOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, OllamaOptions options)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Fail("The name cannot be null or whitespace.");
        }

        if (options is null)
        {
            return Fail("The option cannot be null.");
        }

        if (options.Timeout < Timeout.Infinite)
        {
            return Fail("The timeout value must be a positive number in milliseconds or -1 for an infinite timeout.");
        }

        if (options.Uri is null)
        {
            return Fail("The property Uri cannot be null.");
        }

        return Success;
    }
}
