namespace WebApp.Client.Store.Shared;

public record FetchStartedAction
{
    public string FetchName { get; init; }
    public bool HideLoading { get; init; } = false;
    public bool DispatchErrorToWindow { get; init; } = true;
    public bool ForceDispatch { get; init; } = false;
    public int CacheDurationInMinutes { get; init; } = 5;
}
