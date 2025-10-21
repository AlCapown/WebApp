#nullable enable

using WebApp.Common.Models;

namespace WebApp.Client.Store.Shared;

public record FetchFailureAction
{
    public ApiError? ApiError { get; init; }
    public string? FetchName { get; init; }
    public object? RetryAction { get; init; }
}
