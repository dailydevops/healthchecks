namespace SourceGenerator.HealthChecks;

internal interface ICandidate
{
    string Namespace { get; }
    string Name { get; }

    string OptionsTypeName { get; }
}
