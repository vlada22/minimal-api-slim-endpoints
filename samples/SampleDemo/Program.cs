var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSlimEndpoints();

var app = builder.Build();

app.MapGet("/", (IEnumerable<EndpointDataSource> endpointSources) =>
    string.Join("\n", endpointSources.SelectMany(source => source.Endpoints)));

app.MapSlimEndpoints();

app.Run();

public partial class Program
{
}