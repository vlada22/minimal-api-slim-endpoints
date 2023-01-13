# MinimalApi Slim Endpoints
![Github actions](https://github.com/vlada22/minimal-api-slim-endpoints/actions/workflows/build-release.yml/badge.svg)
[![Nuget feed](https://img.shields.io/nuget/v/MinimalApi.SlimEndpoints?label=MinimalApi.SlimEndpoints)](https://www.nuget.org/packages/MinimalApi.SlimEndpoints)

Small library for decoupling endpoints in [MinimalApi] from the Program.cs file.

This library does not add any additional overhead to the application, it just helps you to organize your code better.

The source generator will generate a DependencyInjection extension that will contain all the endpoints that you have defined in your application.

## Installation
Install the package from NuGet:
```bash
dotnet add package MinimalApi.SlimEndpoints
```

## Quick start

Create a class that inherits from `ISlimEndpoint` and implement the `Configure` and `Handler` methods. Add the `SlimEndpoint` attribute to the class.

### SlimEndpoint attribute
The `SlimEndpoint` attribute has the following properties:
- `Path` - The path of the endpoint. This is the same as the `MapGet` method in the `MinimalApi`.
- `HttpMethod` - The HTTP method of the endpoint. This is the same as the `MapGet` method in the `MinimalApi`.

### Configure method
The `Configure` method injects the `RouteHandlerBuilder` into the endpoint class. Leave the method empty if you don't need to configure the endpoint.

### Handler method
The `Handler` method is the main method of the endpoint. It is called when the endpoint is hit.

### Example
```csharp
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
```

## Dependency Injection configuration
The `MinimalApi.SlimEndpoints` library uses the `Microsoft.Extensions.DependencyInjection` library for dependency injection. You can configure the DI container in the `Program.cs` file.
```csharp
var builder = WebApplication.CreateBuilder(args);

// Register endpoints
builder.Services.AddSlimEndpoints();

var app = builder.Build();

// Map endpoints
app.MapSlimEndpoints();

app.Run();
```

## License
[MIT](https://choosealicense.com/licenses/mit/)

[MinimalApi]: https://devblogs.microsoft.com/aspnet/asp-net-core-updates-in-net-6-preview-4/#introducing-minimal-apis

