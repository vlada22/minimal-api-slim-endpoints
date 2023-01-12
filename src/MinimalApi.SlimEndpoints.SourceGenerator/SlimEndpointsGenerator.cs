using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MinimalApi.SlimEndpoints.SourceGenerator;

[Generator]
public class SlimEndpointsGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var endpoints = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                Constants.EndpointAttributeFullName,
                predicate: (node, _) => node is ClassDeclarationSyntax,
                transform: GetTypeToGenerate)
            .Where(static m => m is not null);

        var compilationAndClasses =
            context.CompilationProvider.Combine(endpoints.Collect());

        context.RegisterSourceOutput(compilationAndClasses,
            static (spc, source) => Execute(source.Right, spc));
    }

    private static EndpointToGenerate? GetTypeToGenerate(GeneratorAttributeSyntaxContext context,
        CancellationToken cancellationToken)
    {
        if (context.TargetSymbol is not INamedTypeSymbol classSymbol)
        {
            // We cannot do anything if the attribute is not a named type
            return default;
        }

        if (context.TargetNode is not ClassDeclarationSyntax classDeclarationSyntax)
        {
            // If the attribute is not on a class, we cannot do anything
            return default;
        }

        if (!classSymbol.AllInterfaces.Any(static i => i.ToDisplayString() == Constants.EndpointInterfaceFullName))
        {
            // If the class does not implement the ISlimEndpoint interface, we cannot do anything
            return default;
        }

        var path = string.Empty;
        var method = string.Empty;

        foreach (var attributeData in classSymbol.GetAttributes())
        {
            if (attributeData.AttributeClass?.ToDisplayString() != Constants.EndpointAttributeFullName)
            {
                continue;
            }

            foreach (var namedArgument in attributeData.NamedArguments)
            {
                switch (namedArgument.Key)
                {
                    case "Path"
                        when namedArgument.Value.Value?.ToString() is { } ns:
                        path = ns;
                        continue;
                    case "Method"
                        when namedArgument.Value.Value?.ToString() is { } n:
                        method = n;
                        break;
                }
            }
        }

        if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(method))
        {
            // If the attribute does not have a path or method, we cannot do anything
            return default;
        }

        // Check if the class is public and partial
        var isPublic = classSymbol.DeclaredAccessibility == Accessibility.Public;
        var isPartial = false;
        foreach (var mod in classDeclarationSyntax.Modifiers)
        {
            if (!mod.IsKind(SyntaxKind.PartialKeyword))
            {
                continue;
            }

            isPartial = true;
            break;
        }

        return new EndpointToGenerate(classSymbol.ContainingNamespace.ToString(),
            classSymbol.Name,
            classSymbol.ToDisplayString(),
            path, method, isPublic, isPartial, classDeclarationSyntax);
    }

    private static void Execute(in ImmutableArray<EndpointToGenerate?> endpointToGenerates,
        SourceProductionContext context)
    {
        // Generate Endpoint classes
        foreach (var endpointToGenerate in endpointToGenerates)
        {
            if (endpointToGenerate is null)
            {
                continue;
            }

            var source = SourceGeneratorParser.GenerateEndpoint(endpointToGenerate.Value, context);
            if (source is null)
            {
                continue;
            }
            
            context.AddSource($"{endpointToGenerate.Value.ClassName}.Routing.g.cs", SourceText.From(source, Encoding.UTF8));
        }
        
        // Generate DI extensions
        var result = SourceGeneratorParser.GenerateExtensionMethods(in endpointToGenerates);
        context.AddSource("SlimEndpointRouteBuilderExtensions.g.cs", SourceText.From(result, Encoding.UTF8));
    }
}