using MinimalApi.SlimEndpoints.Abstractions;

namespace MinimalApi.SlimEndpoints.IntegrationTests.Endpoints;

public partial class TestEndpointNoAttribute : ISlimEndpoint
{
    public void Configure(RouteHandlerBuilder builder)
    {
        
    }

    public Delegate Handler => (()=> "Hello World");
}