#nullable enable

using Fluxor;
using WebApp.Client.Store.Shared;

namespace WebApp.Client.Common.Extensions;

public static class DispatcherExtensions
{
    public static void DispatchFetch(this IDispatcher dispatcher, FetchStartedAction action)
    {
        dispatcher.Dispatch(action with
        {
            FetchName = action.GenerateFetchName()
        });
    }
}
