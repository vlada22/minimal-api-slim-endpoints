using MinimalApi.SlimEndpoints.Abstractions;

namespace MinimalApi.SlimEndpoints.IntegrationTests;

public class SlimEndpointsTests
{
    [Fact]
    public void AddEndpoint()
    {
        var services = new ServiceCollection()
            .AddSlimEndpoints()
            .BuildServiceProvider();

        var endpoints = services
            .GetServices<ISlimEndpoint>();

        Assert.Single(endpoints);
    }

    [Theory]
    [InlineData("/test->GET")]
    public void CheckEndpointPathAndMethodProperties(string expected)
    {
        var services = new ServiceCollection()
            .AddSlimEndpoints()
            .BuildServiceProvider();

        var endpoints = services.GetServices<ISlimEndpoint>();

        Assert.Collection(endpoints, e =>
            Assert.Equal(expected, $"{e.Path}->{e.Method}"));

        //Assert.Equal(expected, $"{endpoint.Path}->{endpoint.Method}");
    }

    [Fact]
    public void MapEndpoint()
    {
        using var host = new HostBuilder()
            .ConfigureWebHost(webHostBuilder =>
            {
                webHostBuilder
                    .UseKestrel()
                    .ConfigureServices(services =>
                    {
                        services.AddConnections();
                        services.AddSlimEndpoints();
                    })
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints => { endpoints.MapSlimEndpoints(); });
                    })
                    .UseUrls("http://127.0.0.1:0");
            })
            .Build();

        host.Start();

        var dataSource = host.Services.GetRequiredService<EndpointDataSource>();

        Assert.Collection(dataSource.Endpoints,
            endpoint => { Assert.Equal("HTTP: GET /test", endpoint.DisplayName); });
    }

    [Fact]
    public void MapEndpointWithOverride()
    {
        using var host = new HostBuilder()
            .ConfigureWebHost(webHostBuilder =>
            {
                webHostBuilder
                    .UseKestrel()
                    .ConfigureServices(services =>
                    {
                        services.AddConnections();
                        services.AddSlimEndpoints();
                    })
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapSlimEndpoints(endpoint => endpoint switch
                            {
                                { Path: "/test" } => "/test2",
                                _ => "/"
                            });
                        });
                    })
                    .UseUrls("http://127.0.0.1:0");
            })
            .Build();

        host.Start();

        var dataSource = host.Services.GetRequiredService<EndpointDataSource>();

        Assert.Collection(dataSource.Endpoints,
            endpoint => { Assert.Equal("HTTP: GET /test2", endpoint.DisplayName); });
    }
}