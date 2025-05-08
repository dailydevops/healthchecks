namespace NetEvolve.HealthChecks.AWS.SNS;

using Microsoft.Extensions.Options;

internal sealed class SimpleNotificationServiceConfigure
    : IConfigureNamedOptions<SimpleNotificationServiceOptions>,
        IValidateOptions<SimpleNotificationServiceOptions>
{
    public void Configure(string? name, SimpleNotificationServiceOptions options) { }

    public void Configure(SimpleNotificationServiceOptions options) { }

    public ValidateOptionsResult Validate(string? name, SimpleNotificationServiceOptions options) =>
        ValidateOptionsResult.Success;
}
