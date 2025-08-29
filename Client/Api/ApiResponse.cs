#nullable enable

namespace WebApp.Client.Api;

public sealed class ApiResponse<TResponse>
{
    public bool IsSuccess { get; init; }
    public int StatusCode { get; init; }
    public TResponse? Response { get; init; }
}
