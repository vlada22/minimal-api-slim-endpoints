using Microsoft.AspNetCore.Builder;

namespace MinimalApi.SlimEndpoints.Abstractions;

public interface ISlimEndpoint
{
    string Path => "/";
    string Method => "GET";
    void Configure(RouteHandlerBuilder builder);
    Delegate Handler { get; }
}