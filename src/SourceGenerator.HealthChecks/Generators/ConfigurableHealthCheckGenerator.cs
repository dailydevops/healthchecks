namespace SourceGenerator.HealthChecks.Generators;

using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using NetEvolve.CodeBuilder;

[Generator(LanguageNames.CSharp)]
internal sealed class ConfigurableHealthCheckGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var toBeImplemented = context
            .SyntaxProvider.ForAttributeWithMetadataName(
                AttributeName.Namespace + AttributeName.ConfigurableHealthCheck,
                static (s, _) => IsTargetForGeneration(s),
                static (ctx, _) => GetCandidate(ctx)!
            )
            .Where(c => c is not null);
        context.RegisterSourceOutput(toBeImplemented, static (ctx, candidate) => Generate(ctx, candidate));
    }

    private static void Generate(SourceProductionContext context, Candidate candidate)
    {
        var codeBuilder = CodeGenerator.ConfigurableHealthCheck(
            candidate,
            GenerateHelperMethods,
            withWin32ExceptionHandling: candidate.IncludeWin32Handling,
            includeServiceProvider: true
        );
        context.AddSource($"{candidate.Name}.g.cs", SourceText.From(codeBuilder.ToString(), Encoding.UTF8));
    }

    private static void GenerateHelperMethods(CSharpCodeBuilder builder)
    {
        _ = builder
            .AppendLine()
            .AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]")
            .AppendLine("private static HealthCheckResult HealthCheckState(bool condition, string name) =>")
            .Intend()
            .AppendLine("condition ? HealthCheckResult.Healthy($\"{name}: Healthy\") : HealthCheckDegraded(name);");

        _ = builder
            .AppendLine()
            .AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]")
            .AppendLine(
                "private static HealthCheckResult HealthCheckUnhealthy(HealthStatus failureStatus, string name, string message = \"Unhealthy\", Exception? ex = null) =>"
            )
            .Intend()
            .AppendLine("new HealthCheckResult(failureStatus, $\"{name}: {message}\", exception: ex);");

        _ = builder
            .AppendLine()
            .AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]")
            .AppendLine("private static HealthCheckResult HealthCheckDegraded(string name) =>")
            .Intend()
            .AppendLine("HealthCheckResult.Degraded($\"{name}: Degraded\");");
    }

    private static Candidate? GetCandidate(GeneratorAttributeSyntaxContext context)
    {
        if (context.TargetNode is not ClassDeclarationSyntax classDeclaration)
        {
            return null;
        }
        var attributeData = context.Attributes.FirstOrDefault(a =>
            a.AttributeClass!.Name == AttributeName.ConfigurableHealthCheck
        );
        if (attributeData is null)
        {
            return null;
        }
        return GetCandidateInformation(context.SemanticModel, classDeclaration, attributeData);
    }

    private static Candidate? GetCandidateInformation(
        SemanticModel semanticModel,
        ClassDeclarationSyntax classDeclaration,
        AttributeData attributeData
    )
    {
        if (attributeData.ConstructorArguments.Length != 2)
        {
            return null;
        }

        if (attributeData.ConstructorArguments[0].Value is not ISymbol optionsType)
        {
            return null;
        }

        if (semanticModel.GetDeclaredSymbol(classDeclaration) is not ISymbol classSymbol)
        {
            return null;
        }

        return new Candidate
        {
            Namespace = classSymbol.ContainingNamespace.ToDisplayString(),
            Name = classDeclaration.Identifier.Text,
            OptionsTypeName = optionsType.Name,
            OptionsTypeNamespace = optionsType.ContainingNamespace.ToDisplayString(),
            IncludeWin32Handling = attributeData.ConstructorArguments[1].Value is true,
        };
    }

    private static bool IsTargetForGeneration(SyntaxNode node) =>
        node is ClassDeclarationSyntax { AttributeLists.Count: > 0 } classNode
        && classNode.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));

    private sealed record Candidate : ICandidate
    {
        public string Namespace { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string OptionsTypeName { get; internal set; } = default!;
        public string OptionsTypeNamespace { get; internal set; } = default!;

        public bool IncludeWin32Handling { get; set; }
    }
}
