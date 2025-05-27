namespace NetEvolve.HealthChecks.Tests.Integration.Internals;

/// <summary>
/// An attribute that marks a test method as non-executable, providing a reason for skipping the test.
/// </summary>
/// <remarks>
/// This attribute is a custom extension of <see cref="FactAttribute"/>. It sets the <see cref="FactAttribute.Skip"/> property
/// to the provided reason, which indicates why the test is not executable.
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class NotExecutableFactAttribute : FactAttribute
{
    public string Reason { get; }

    public NotExecutableFactAttribute(string reason) => Skip = Reason = reason;
}
