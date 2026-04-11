#nullable enable

using WebApp.Common.Models;

namespace WebApp.Client.Store.Shared;

public record FetchFailureAction
{
    public required ApiError ApiError { get; init; }
    public object? RetryAction { get; init; }
}
