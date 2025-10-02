#nullable enable

namespace WebApp.Client.Store.Shared;

public record FetchStartedAction
{
    public string? FetchName { get; init; }
    public FetchOptions FetchOptions { get; init; } = FetchOptions.Default;
    public int CacheDurationInMinutes { get; init; } = 5;
}
