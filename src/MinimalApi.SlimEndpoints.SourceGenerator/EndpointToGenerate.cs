using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MinimalApi.SlimEndpoints.SourceGenerator;

public readonly record struct EndpointToGenerate(string Namespace, string ClassName, string ClassFullName, string Path, string Method, bool IsPublic, bool IsPartial, ClassDeclarationSyntax EndpointClassDeclarationSyntax)
{
    public string Namespace { get; } = Namespace;
    public string ClassName { get; } = ClassName;
    public string ClassFullName { get; } = ClassFullName;
    public string Path { get; } = Path;
    public string Method { get; } = Method;
    public bool IsPublic { get; } = IsPublic;
    public bool IsPartial { get; } = IsPartial;
    public ClassDeclarationSyntax EndpointClassDeclarationSyntax { get; } = EndpointClassDeclarationSyntax;
}