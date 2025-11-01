namespace NetEvolve.HealthChecks.GCP.Firestore;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class FirestoreOptionsConfigure
    : IConfigureNamedOptions<FirestoreOptions>,
        IValidateOptions<FirestoreOptions>
{
    private readonly IConfiguration _configuration;

    public FirestoreOptionsConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, FirestoreOptions options)
    {
        ArgumentNullException.ThrowIfNull(name);

        _configuration.Bind($"HealthChecks:GCP:Firestore:{name}", options);
    }

    public void Configure(FirestoreOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, FirestoreOptions options)
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

        return Success;
    }
}
