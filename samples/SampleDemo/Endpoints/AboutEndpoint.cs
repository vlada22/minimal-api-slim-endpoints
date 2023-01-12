using MinimalApi.SlimEndpoints.Abstractions;

namespace SampleDemo.Endpoints;

[SlimEndpoint(Path = "/about", Method = "GET")]
public partial class AboutEndpoint : ISlimEndpoint
{
    public void Configure(RouteHandlerBuilder builder)
    {
        builder.AllowAnonymous();
    }

    public Delegate Handler => (() => Results.Ok("about"));
}