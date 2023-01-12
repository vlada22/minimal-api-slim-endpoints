namespace MinimalApi.SlimEndpoints.Abstractions;

[AttributeUsage(System.AttributeTargets.Class)]
public class SlimEndpointAttribute : Attribute
{
    public string Path { get; set; } = "/";
    public string Method { get; set; } = "GET";
}