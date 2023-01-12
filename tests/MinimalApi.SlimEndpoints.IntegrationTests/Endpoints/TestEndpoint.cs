using MinimalApi.SlimEndpoints.Abstractions;

namespace MinimalApi.SlimEndpoints.IntegrationTests.Endpoints;

[SlimEndpoint(Path = "/test", Method = "GET")]
public partial class TestEndpoint : ISlimEndpoint
{
    public void Configure(RouteHandlerBuilder builder)
    {
        builder.AllowAnonymous();
    }

    public Delegate Handler => (()=> "Hello World");
}