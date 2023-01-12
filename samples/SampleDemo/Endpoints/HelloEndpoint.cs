using MinimalApi.SlimEndpoints.Abstractions;

namespace SampleDemo.Endpoints;

[SlimEndpoint(Path = "/hello", Method = "GET")]
public partial class HelloEndpoint : ISlimEndpoint
{
    public void Configure(RouteHandlerBuilder builder)
    {
        builder.AllowAnonymous();
    }

    public Delegate Handler => (() => "Hello World!");
}