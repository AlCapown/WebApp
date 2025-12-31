#nullable enable

using Fluxor;
using System;
using WebApp.Client.Store.FetchStore;
using WebApp.Client.Store.PageStore;
using WebApp.Common.Extensions;
using WebApp.Common.Models;

namespace WebApp.Client.Store.Shared;

internal sealed class FetchMiddleware : Middleware
{
    private readonly IState<FetchState> _fetchState;
    private readonly IDispatcher _dispatcher;

    public FetchMiddleware(IState<FetchState> fetchState, IDispatcher dispatcher)
    {
        _fetchState = fetchState;
        _dispatcher = dispatcher;
    }

    public override bool MayDispatchAction(object action)
    {
        if (action is FetchStartedAction fetchStartedAction)
        {
            return HandleFetchStartedDispatch(fetchStartedAction);
        }
        
        return true;
    }

    private bool HandleFetchStartedDispatch(FetchStartedAction fetchStartedAction)
    {
        if (fetchStartedAction.FetchName.IsNullOrWhiteSpace())
        {
            _dispatcher.Dispatch(new PageActions.EnqueuePageError
            {
                Error = new LocalError
                {
                    Message = $"A {nameof(FetchStartedAction)} was dispatched without a FetchName specified."
                }
            });

            return false;
        }
        
        bool shouldDispatch = true;

        if (!fetchStartedAction.FetchOptions.HasFlag(FetchOptions.ForceDispatch) 
            && _fetchState.Value.Fetches.TryGetValue(fetchStartedAction.FetchName, out var fetch))
        {
            shouldDispatch = fetch.IsLoading == false && (fetch.CacheExpires == null || fetch.CacheExpires < DateTimeOffset.Now);
        }

        if (shouldDispatch)
        {
            _dispatcher.Dispatch(new FetchActions.FetchStarted
            {
                FetchName = fetchStartedAction.FetchName,
                HideLoading = fetchStartedAction.FetchOptions.HasFlag(FetchOptions.HideLoading)
            });
        }

        return shouldDispatch;
    }
}
