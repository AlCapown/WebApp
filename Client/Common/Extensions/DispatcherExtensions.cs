#nullable enable

using Fluxor;
using WebApp.Client.Store.Shared;

namespace WebApp.Client.Common.Extensions;

public static class DispatcherExtensions
{
    public static string DispatchFetch<TAction>(this IDispatcher dispatcher, TAction action)
        where TAction : FetchStartedAction
    {
        string fetchName = $"{action.GetType().Name}_{action.GetHashCode()}";

        dispatcher.Dispatch(action with
        {
            FetchName = fetchName
        });

        return fetchName;
    }
}
