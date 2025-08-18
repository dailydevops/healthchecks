namespace NetEvolve.HealthChecks.Azure.DigitalTwin;

using System;
using global::Azure.DigitalTwins.Core;

internal interface IDigitalTwinOptions
{
    Uri? ServiceUri { get; }

    DigitalTwinClientCreationMode? Mode { get; }

    Action<DigitalTwinsClientOptions>? ConfigureClientOptions { get; }

    int Timeout { get; }
}