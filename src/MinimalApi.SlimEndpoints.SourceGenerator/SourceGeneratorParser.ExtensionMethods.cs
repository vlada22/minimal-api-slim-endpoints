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
        sb.Append(@"
");
        AddEndpoints(sb);
        sb.Append(@"
");

        sb.Append(@"
        private static RouteHandlerBuilder GenerateHttpMethod(")
            .Append(Constants.EndpointInterfaceFullName).Append(" endpoint,")
            .Append(" IEndpointRouteBuilder builder, string path) => endpoint.Method switch")
            .Append("""
        {
            "GET" => builder.MapGet(path, endpoint.Handler),
            "POST" => builder.MapPost(path, endpoint.Handler),
            "PUT" => builder.MapPut(path, endpoint.Handler),
            "DELETE" => builder.MapDelete(path, endpoint.Handler),
            "PATCH" => builder.MapPatch(path, endpoint.Handler),
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
        /// <summary>
        /// Maps decorated endpoints to the specified <see cref=""IEndpointRouteBuilder""/>.
        /// </summary>
        /// <param name=""endpoints""></param>
        /// <returns></returns>
        public static IEndpointRouteBuilder MapSlimEndpoints(this IEndpointRouteBuilder endpoints) => endpoints.MapSlimEndpoints(_ => default);");

        sb.Append(@"
");
        
        sb.Append(@"
        /// <summary>
        /// Maps decorated endpoints to the specified <see cref=""IEndpointRouteBuilder""/>.
        /// With the ability to configure function for path override of the endpoints.
        /// </summary>
        /// <param name=""endpoints""></param>
        /// <param name=""routeSelector""></param>
        /// <returns></returns>
        public static IEndpointRouteBuilder MapSlimEndpoints(this IEndpointRouteBuilder endpoints, Func<")
            .Append(Constants.EndpointInterfaceFullName).Append(", string?> routeSelector)")
            .Append(@"
        {");

        sb.Append(@"
            var slimEndpoints = endpoints.ServiceProvider.GetServices<")
            .Append(Constants.EndpointInterfaceFullName)
            .Append(">();");

        sb.Append(@"
            foreach (var slimEndpoint in slimEndpoints)
            {
                slimEndpoint.Configure(GenerateHttpMethod(slimEndpoint, endpoints, routeSelector(slimEndpoint) ?? slimEndpoint.Path));
            }");
        
        sb.Append(@"
            return endpoints;
        }");
    }
}