using MinimalApi.SlimEndpoints.Abstractions;

namespace MinimalApi.SlimEndpoints.IntegrationTests.Endpoints;

[SlimEndpoint(Path = "/test1", Method = "GET")]
public partial class TestEndpointNoInterface
{
    
}