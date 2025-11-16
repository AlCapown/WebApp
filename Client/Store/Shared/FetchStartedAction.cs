#nullable enable

namespace WebApp.Client.Store.Shared;

public record FetchStartedAction
{
    public string? FetchName { get; init; }
    public FetchOptions FetchOptions { get; init; } = FetchOptions.Default;
    public int CacheDurationInMinutes { get; init; } = 5;
}


public static class FetchStartedExtensions
{
    public static string GenerateFetchName(this FetchStartedAction action)
    {
        return $"{action.GetType().Name}_{action.GetHashCode()}";
    }
}