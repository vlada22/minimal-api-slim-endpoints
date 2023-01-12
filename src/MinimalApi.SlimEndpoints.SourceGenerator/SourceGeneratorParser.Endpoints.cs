using System.Text;
using Microsoft.CodeAnalysis;

namespace MinimalApi.SlimEndpoints.SourceGenerator;

public static partial class SourceGeneratorParser
{
    public static string? GenerateEndpoint(in EndpointToGenerate endpointToGenerate, SourceProductionContext context)
    {
        if (!endpointToGenerate.IsPublic)
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.EndpointClassMustBePublic, endpointToGenerate.EndpointClassDeclarationSyntax.GetLocation()));
            return default;
        }

        if (!endpointToGenerate.IsPartial)
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.EndpointClassMustBePartial, endpointToGenerate.EndpointClassDeclarationSyntax.GetLocation()));
            return default;
        }

        var sb = new StringBuilder();

        sb.Append(Header);
        sb.Append(@"
namespace ").Append(endpointToGenerate.Namespace).Append(@"
{");
        sb.Append(@"
    public partial class ").Append(endpointToGenerate.ClassName).Append(@"
    {
        public string Path => """).Append(endpointToGenerate.Path).Append("\";").Append(@"
        public string Method => """).Append(endpointToGenerate.Method).Append("\";").Append(@"
    }
}");

        return sb.ToString();
    }
}