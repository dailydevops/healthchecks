namespace NetEvolve.HealthChecks.Tests.Integration.Internals;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class NotExecutableFactAttribute : FactAttribute
{
    public string Reason { get; }

    public NotExecutableFactAttribute(string reason) => Skip = Reason = reason;
}
