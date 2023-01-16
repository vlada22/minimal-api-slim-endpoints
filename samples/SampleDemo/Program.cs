using SampleDemo;
using SampleDemo.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ExampleService>();

builder.Services.AddSlimEndpoints();

var app = builder.Build();

app.MapGet("/", (IEnumerable<EndpointDataSource> endpointSources) =>
    string.Join("\n", endpointSources.SelectMany(source => source.Endpoints)));

// map endpoints without routes override
// app.MapSlimEndpoints();

// possibility to override endpoint routes
app.MapSlimEndpoints((endpoint)=> endpoint.Path switch
{
    // override route /hello to /hello1
    "/hello" => "/hello1",
    // leave other routes as is
    _ => default
});

app.Run();