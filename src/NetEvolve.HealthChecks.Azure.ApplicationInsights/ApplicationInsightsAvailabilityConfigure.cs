namespace NetEvolve.HealthChecks.Azure.ApplicationInsights;

using System;
using System.Threading;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class ApplicationInsightsAvailabilityConfigure
    : IConfigureNamedOptions<ApplicationInsightsAvailabilityOptions>,
        IValidateOptions<ApplicationInsightsAvailabilityOptions>
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public ApplicationInsightsAvailabilityConfigure(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    public void Configure(string? name, ApplicationInsightsAvailabilityOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:ApplicationInsightsAvailability:{name}", options);
    }

    public void Configure(ApplicationInsightsAvailabilityOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, ApplicationInsightsAvailabilityOptions options)
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

        var mode = options.Mode;

        return options.Mode switch
        {
            ApplicationInsightsClientCreationMode.ServiceProvider => ValidateModeServiceProvider(),
            ApplicationInsightsClientCreationMode.ConnectionString => ValidateModeConnectionString(options),
            ApplicationInsightsClientCreationMode.InstrumentationKey => ValidateModeInstrumentationKey(options),
            _ => Fail($"The mode `{mode}` is not supported."),
        };
    }

    private static ValidateOptionsResult ValidateModeConnectionString(ApplicationInsightsAvailabilityOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            return Fail(
                $"The connection string cannot be null or whitespace when using `{nameof(ApplicationInsightsClientCreationMode.ConnectionString)}` mode."
            );
        }

        return Success;
    }

    private static ValidateOptionsResult ValidateModeInstrumentationKey(ApplicationInsightsAvailabilityOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.InstrumentationKey))
        {
            return Fail(
                $"The instrumentation key cannot be null or whitespace when using `{nameof(ApplicationInsightsClientCreationMode.InstrumentationKey)}` mode."
            );
        }

        return Success;
    }

    private ValidateOptionsResult ValidateModeServiceProvider()
    {
        if (_serviceProvider.GetService<TelemetryClient>() is null)
        {
            return Fail(
                $"No service of type `{nameof(TelemetryClient)}` registered. Please register Application Insights using AddApplicationInsightsTelemetry()."
            );
        }

        return Success;
    }
}
