namespace SourceGenerator.HealthChecks;

using NetEvolve.CodeBuilder;

/// <summary>
/// Helper extensions for <see cref="CSharpCodeBuilder"/> to generate HealthCheckResult code.
/// </summary>
public static class CodeBuilderExtensions
{
    /// <summary>
    /// Appends code to return a degraded health check result.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="builder"/> is <see langword="null"/>.
    /// </exception>
    public static CSharpCodeBuilder HealthCheckDegraded(this CSharpCodeBuilder builder) =>
        builder?.AppendLine("return HealthCheckResult.Degraded($\"{name}: Degraded\");")
        ?? throw new ArgumentNullException(nameof(builder));

    /// <summary>
    /// Appends code to return a healthy health check result.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="builder"/> is <see langword="null"/>.
    /// </exception>
    public static CSharpCodeBuilder HealthCheckHealthy(this CSharpCodeBuilder builder) =>
        builder?.AppendLine("return HealthCheckResult.Healthy($\"{name}: Healthy\");")
        ?? throw new ArgumentNullException(nameof(builder));

    /// <summary>
    /// Appends code to return a healthy health check result.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="builder"/> is <see langword="null"/>.
    /// </exception>
    public static CSharpCodeBuilder HealthCheckState(this CSharpCodeBuilder builder, string condition) =>
        builder
            ?.Append($"return {condition} ? ")
            .AppendLine(
                "HealthCheckResult.Healthy($\"{name}: Healthy\") : HealthCheckResult.Degraded($\"{name}: Degraded\");"
            )
        ?? throw new ArgumentNullException(nameof(builder));

    /// <summary>
    /// Appends code to return a healthy health check result as a ValueTask.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="builder"/> is <see langword="null"/>.
    /// </exception>
    public static CSharpCodeBuilder HealthCheckStateValueTask(this CSharpCodeBuilder builder, string condition) =>
        builder
            ?.Append($"return ValueTask.FromResult({condition} ? ")
            .AppendLine(
                "HealthCheckResult.Healthy($\"{name}: Healthy\") : HealthCheckResult.Degraded($\"{name}: Degraded\"));"
            )
        ?? throw new ArgumentNullException(nameof(builder));

    /// <summary>
    /// Appends code to return an unhealthy health check result.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="builder"/> is <see langword="null"/>.
    /// </exception>
    public static CSharpCodeBuilder HealthCheckUnhealthy(
        this CSharpCodeBuilder builder,
        string message,
        string? exceptionName = null
    ) =>
        builder
            ?.Append($"return new HealthCheckResult(failureStatus, $\"{{name}}: {message}\"")
            .AppendIf(!string.IsNullOrWhiteSpace(exceptionName), $", exception: {exceptionName}")
            .AppendLine(");")
        ?? throw new ArgumentNullException(nameof(builder));
}
