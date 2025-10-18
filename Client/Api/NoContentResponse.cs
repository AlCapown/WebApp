#nullable enable

namespace WebApp.Client.Api;

public sealed class NoContentResponse
{
    private static readonly NoContentResponse _instance = new();
    private NoContentResponse() { }
    public static NoContentResponse Value  => _instance;
}
