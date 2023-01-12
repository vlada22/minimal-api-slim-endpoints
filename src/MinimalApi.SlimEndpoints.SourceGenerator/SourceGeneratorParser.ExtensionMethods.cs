using System.Collections.Immutable;
using System.Text;

namespace MinimalApi.SlimEndpoints.SourceGenerator;

public static partial class SourceGeneratorParser
{
    public static string GenerateExtensionMethods(in ImmutableArray<EndpointToGenerate?> endpointToGenerates)
    {
        var sb = new StringBuilder();
        
        sb.Append(Header);
        sb.Append(@"
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
namespace Microsoft.Extensions.DependencyInjection
{
    public static class SlimEndpointRouteBuilderExtensions
    {");

        AddServices(sb, endpointToGenerates);
        AddEndpoints(sb);

        sb.Append(@"
        private static RouteHandlerBuilder GenerateHttpMethod(")
            .Append(Constants.EndpointInterfaceFullName).Append(" endpoint,")
            .Append("""
            IEndpointRouteBuilder builder) => endpoint.Method switch
        {
            "GET" => builder.MapGet(endpoint.Path, endpoint.Handler),
            "POST" => builder.MapPost(endpoint.Path, endpoint.Handler),
            "PUT" => builder.MapPut(endpoint.Path, endpoint.Handler),
            "DELETE" => builder.MapDelete(endpoint.Path, endpoint.Handler),
            "PATCH" => builder.MapPatch(endpoint.Path, endpoint.Handler),
            _ => throw new ArgumentOutOfRangeException(nameof(endpoint.Method), "Invalid HTTP method")
        };
    }
}
""");

        return sb.ToString();
    }

    private static void AddServices(StringBuilder sb, in ImmutableArray<EndpointToGenerate?> endpointToGenerates)
    {
        sb.Append(@"
        public static IServiceCollection AddSlimEndpoints(this IServiceCollection services)
        {");
        
        foreach (var endpointToGenerate in endpointToGenerates)
        {
            if (endpointToGenerate is null)
            {
                continue;
            }

            sb.Append(@"
            services.TryAddEnumerable(ServiceDescriptor.Transient<")
                .Append(Constants.EndpointInterfaceFullName)
                .Append(", ")
                .Append(endpointToGenerate.Value.ClassFullName)
                .AppendLine(">());");
            
        }
        
        sb.Append(@"
            return services;
        }");
    }
    
    private static void AddEndpoints(StringBuilder sb)
    {
        sb.Append(@"
        public static IEndpointRouteBuilder MapSlimEndpoints(this IEndpointRouteBuilder endpoints)
        {");

        sb.Append(@"
            var slimEndpoints = endpoints.ServiceProvider.GetServices<")
            .Append(Constants.EndpointInterfaceFullName)
            .Append(">();");

        sb.Append(@"
            foreach (var slimEndpoint in slimEndpoints)
            {
                slimEndpoint.Configure(GenerateHttpMethod(slimEndpoint, endpoints));
            }");
        
        sb.Append(@"
            return endpoints;
        }");
    }
}