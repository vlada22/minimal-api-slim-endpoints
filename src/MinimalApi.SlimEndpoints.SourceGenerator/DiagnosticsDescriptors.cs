using Microsoft.CodeAnalysis;

namespace MinimalApi.SlimEndpoints.SourceGenerator;

public static class DiagnosticsDescriptors
{
    public static DiagnosticDescriptor EndpointClassMustBePartial { get; } = new(
        id: "SELIB1001",
        title: "Endpoint class must be partial",
        messageFormat: "Endpoint class must be partial",
        category: "SlimEndpointsGenerator",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);
    
    public static DiagnosticDescriptor EndpointClassMustBePublic { get; } = new(
        id: "SELIB1002",
        title: "Endpoint class must be public",
        messageFormat: "Endpoint class must be public",
        category: "SlimEndpointsGenerator",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}