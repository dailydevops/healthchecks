namespace NetEvolve.HealthChecks.Apache.ActiveMq;

using Microsoft.Extensions.Options;

internal sealed class ActiveMqConfigure
    : IConfigureNamedOptions<ActiveMqOptions>,
        IValidateOptions<ActiveMqOptions>
{
    public void Configure(string? name, ActiveMqOptions options) { }

    public void Configure(ActiveMqOptions options) { }

    public ValidateOptionsResult Validate(string? name, ActiveMqOptions options) =>
        ValidateOptionsResult.Success;
}
