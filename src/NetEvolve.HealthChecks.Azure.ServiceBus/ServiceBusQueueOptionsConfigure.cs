namespace NetEvolve.HealthChecks.Azure.ServiceBus;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal class ServiceBusQueueOptionsConfigure
    : IConfigureNamedOptions<ServiceBusQueueOptions>,
        IValidateOptions<ServiceBusQueueOptions>
{
    private readonly IConfiguration _configuration;

    public ServiceBusQueueOptionsConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, ServiceBusQueueOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:AzureServiceBusQueue:{name}", options);
    }

    public void Configure(ServiceBusQueueOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, ServiceBusQueueOptions options)
    {
        var result = ServiceBusOptionsBase.InternalValidate(name, options);

        if (result is not null)
        {
            return result;
        }

        if (string.IsNullOrWhiteSpace(options.QueueName))
        {
            return Fail("The queue name cannot be null or whitespace.");
        }

        return Success;
    }
}
