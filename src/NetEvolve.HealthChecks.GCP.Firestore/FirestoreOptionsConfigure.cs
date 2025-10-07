namespace NetEvolve.HealthChecks.GCP.Firestore;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

internal sealed class FirestoreOptionsConfigure
    : IConfigureNamedOptions<FirestoreOptions>,
        IPostConfigureOptions<FirestoreOptions>
{
    private readonly IConfiguration _configuration;

    public FirestoreOptionsConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, FirestoreOptions options)
    {
        ArgumentNullException.ThrowIfNull(name);

        _configuration.Bind($"HealthChecks:GCP:Firestore:{name}", options);
    }

    public void Configure(FirestoreOptions options) => Configure(Options.DefaultName, options);

    public void PostConfigure(string? name, FirestoreOptions options)
    {
        if (options.Timeout < -1)
        {
            options.Timeout = -1;
        }
    }
}
