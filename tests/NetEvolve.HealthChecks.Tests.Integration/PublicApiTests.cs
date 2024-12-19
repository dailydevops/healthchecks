namespace NetEvolve.HealthChecks.Tests.Integration;

using System.ComponentModel;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using PublicApiGenerator;

public class PublicApiTests
{
    private static readonly string[] _excludedAttributes =
    [
        typeof(InternalsVisibleToAttribute).FullName!,
        "System.Runtime.CompilerServices.IsByRefLikeAttribute",
        typeof(TargetFrameworkAttribute).FullName!,
        typeof(CLSCompliantAttribute).FullName!,
        typeof(AssemblyMetadataAttribute).FullName!,
        typeof(NeutralResourcesLanguageAttribute).FullName!,
        typeof(AttributeUsageAttribute).FullName!,
    ];

    [Theory]
    [MemberData(nameof(GetAssemblies))]
    public Task PublicApi_HasNotChanged_Theory(Assembly assembly)
    {
        Assert.NotNull(assembly);

        var types = assembly.GetTypes().Where(IsVisibleToIntelliSense).ToArray();

        var options = new ApiGeneratorOptions
        {
            ExcludeAttributes = _excludedAttributes,
            IncludeTypes = types,
        };

        var publicApi = assembly.GeneratePublicApi(options);

        return Verify(publicApi).UseTypeName(assembly.GetName().Name);
    }

    public static TheoryData<Assembly> GetAssemblies
    {
        get
        {
            var assemblies = Assembly
                .GetExecutingAssembly()!
                .GetReferencedAssemblies()
                .Where(a =>
                    a.Name?.StartsWith("NetEvolve.HealthChecks", StringComparison.OrdinalIgnoreCase)
                    == true
                )
                .Select(Assembly.Load)
                .ToArray();

            var data = new TheoryData<Assembly>();
            data.AddRange(assemblies);
            return data;
        }
    }

    private static bool IsVisibleToIntelliSense(Type type)
    {
        var browsable = type.GetCustomAttribute<BrowsableAttribute>();
        if (browsable is null || browsable.Browsable)
        {
            return true;
        }

        var editorBrowsable = type.GetCustomAttribute<EditorBrowsableAttribute>();
        return editorBrowsable is null || editorBrowsable.State != EditorBrowsableState.Never;
    }
}
