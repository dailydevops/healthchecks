﻿namespace NetEvolve.HealthChecks.Azure.Tables;

using System;
using System.Threading;
using global::Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class TableServiceAvailableConfigure
    : IConfigureNamedOptions<TableServiceAvailableOptions>,
        IValidateOptions<TableServiceAvailableOptions>
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public TableServiceAvailableConfigure(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    public void Configure(string? name, TableServiceAvailableOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:AzureTableService:{name}", options);
    }

    public void Configure(TableServiceAvailableOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, TableServiceAvailableOptions options)
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
            TableClientCreationMode.ServiceProvider => ValidateModeServiceProvider(),
            TableClientCreationMode.ConnectionString => ValidateModeConnectionString(options),
            TableClientCreationMode.DefaultAzureCredentials => ValidateModeDefaultAzureCredentials(options),
            TableClientCreationMode.SharedKey => ValidateModeSharedKey(options),
            TableClientCreationMode.AzureSasCredential => ValidateModeAzureSasCredential(options),
            _ => Fail($"The mode `{mode}` is not supported."),
        };
    }

    private static ValidateOptionsResult ValidateModeAzureSasCredential(TableServiceAvailableOptions options)
    {
        if (options.ServiceUri is null)
        {
            return Fail(
                $"The service url cannot be null when using `{nameof(TableClientCreationMode.AzureSasCredential)}` mode."
            );
        }

        if (!options.ServiceUri.IsAbsoluteUri)
        {
            return Fail(
                $"The service url must be an absolute url when using `{nameof(TableClientCreationMode.AzureSasCredential)}` mode."
            );
        }

        if (string.IsNullOrWhiteSpace(options.ServiceUri.Query))
        {
            return Fail(
                $"The sas query token cannot be null or whitespace when using `{nameof(TableClientCreationMode.AzureSasCredential)}` mode."
            );
        }

        return Success;
    }

    private static ValidateOptionsResult ValidateModeSharedKey(TableServiceAvailableOptions options)
    {
        if (options.ServiceUri is null)
        {
            return Fail(
                $"The service url cannot be null when using `{nameof(TableClientCreationMode.SharedKey)}` mode."
            );
        }

        if (!options.ServiceUri.IsAbsoluteUri)
        {
            return Fail(
                $"The service url must be an absolute url when using `{nameof(TableClientCreationMode.SharedKey)}` mode."
            );
        }

        if (string.IsNullOrWhiteSpace(options.AccountName))
        {
            return Fail(
                $"The account name cannot be null or whitespace when using `{nameof(TableClientCreationMode.SharedKey)}` mode."
            );
        }

        if (string.IsNullOrWhiteSpace(options.AccountKey))
        {
            return Fail(
                $"The account key cannot be null or whitespace when using `{nameof(TableClientCreationMode.SharedKey)}` mode."
            );
        }

        return Success;
    }

    private static ValidateOptionsResult ValidateModeDefaultAzureCredentials(TableServiceAvailableOptions options)
    {
        if (options.ServiceUri is null)
        {
            return Fail(
                $"The service url cannot be null when using `{nameof(TableClientCreationMode.DefaultAzureCredentials)}` mode."
            );
        }

        if (!options.ServiceUri.IsAbsoluteUri)
        {
            return Fail(
                $"The service url must be an absolute url when using `{nameof(TableClientCreationMode.DefaultAzureCredentials)}` mode."
            );
        }

        return Success;
    }

    private static ValidateOptionsResult ValidateModeConnectionString(TableServiceAvailableOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            return Fail(
                $"The connection string cannot be null or whitespace when using `{nameof(TableClientCreationMode.ConnectionString)}` mode."
            );
        }

        return Success;
    }

    private ValidateOptionsResult ValidateModeServiceProvider()
    {
        if (_serviceProvider.GetService<TableServiceClient>() is null)
        {
            return Fail(
                $"No service of type `{nameof(TableServiceClient)}` registered. Please execute `builder.AddAzureClients()`."
            );
        }

        return Success;
    }
}
