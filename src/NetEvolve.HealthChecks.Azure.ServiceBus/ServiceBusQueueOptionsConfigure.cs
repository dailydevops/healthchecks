namespace NetEvolve.HealthChecks.Azure.ServiceBus;

using Microsoft.Extensions.Options;

internal class ServiceBusQueueOptionsConfigure
    : IConfigureNamedOptions<ServiceBusQueueOptions>,
        IValidateOptions<ServiceBusQueueOptions>
{
    public void Configure(string? name, ServiceBusQueueOptions options) { }

    public void Configure(ServiceBusQueueOptions options) { }

    public ValidateOptionsResult Validate(string? name, ServiceBusQueueOptions options) =>
        ValidateOptionsResult.Success;
}
