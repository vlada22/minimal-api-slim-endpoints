using MinimalApi.SlimEndpoints.Abstractions;
using SampleDemo.Services;

namespace SampleDemo.Endpoints;

[SlimEndpoint(Path = "/hello", Method = "GET")]
public partial class HelloEndpoint : ISlimEndpoint
{
    private readonly ILogger<HelloEndpoint> _logger;

    public HelloEndpoint(ILogger<HelloEndpoint> logger)
    {
        _logger = logger;
    }

    public void Configure(RouteHandlerBuilder builder)
    {
        _logger.LogInformation("Configuring HelloEndpoint");
        
        builder.AllowAnonymous();
    }

    // Please Note: You should not capture injected services from the constructor in the handler delegate
    // Instead, you should inject them into the handler method
    // For example, if you want to use ILogger, you should inject it into the handler method, not the _logger field injected from the constructor
    public Delegate Handler => (ExampleService service, ILogger<HelloEndpoint> log) =>
    {
        log.LogInformation("HelloEndpoint is being executed");

        return service.GetExample();
    };
}