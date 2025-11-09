#nullable enable

using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using WebApp.Client.Common.Extensions;
using WebApp.Client.Store.FetchStore;
using WebApp.Client.Store.PageStore;
using WebApp.Client.Store.Shared;

namespace WebApp.Client.Components.Common.WebAppComponentBase;

public class WebAppComponentBase : FluxorComponent
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = default!;

    [Inject]
    private IState<FetchState> FetchState { get; set; } = default!;

    private readonly HashSet<string> _fetches;
    private readonly List<EventHandler> _chainedEventHandlers;

    public WebAppComponentBase() : base() 
    {
        _fetches = [];
        _chainedEventHandlers = [];
    }

    /// <summary>
    /// Dispatches a <see cref="FetchStartedAction"/>. Custom middleware will dispatch the action only if 
    /// it the action as never been dispatched before or if the store cache has expired for this action.
    /// All actions are individually tracked so their loading state can be read easily using <see cref="IsLoading"/>.
    /// Actions can be chained together so that when one action completes, the next action is dispatched automatically.
    /// </summary>
    /// <param name="action">The initial fetch action to dispatch.</param>
    /// <param name="nextActions">
    /// Optional factory functions that return the subsequent actions to dispatch in sequence. Actions are 
    /// executed in order, and each factory function is invoked only after the previous action completes.
    /// If any action factory returns null, the sequence terminates at that point.
    /// </param>
    public void MaybeDispatch(FetchStartedAction action, params Func<FetchStartedAction?>[] nextActions)
    {
        string fetchName = Dispatcher.DispatchFetch(action);
        _fetches.Add(fetchName);
        StateHasChanged();

        FetchState.StateChanged += OnStateChangedForChainedAction;
        _chainedEventHandlers.Add(OnStateChangedForChainedAction);

        void OnStateChangedForChainedAction(object? sender, EventArgs e)
        {
            if (FetchState.Value.Fetches.TryGetValue(fetchName, out var fetch)
                && fetch is not null
                && fetch.IsComplete)
            {
                FetchState.StateChanged -= OnStateChangedForChainedAction;
                _chainedEventHandlers.Remove(OnStateChangedForChainedAction);

                if (nextActions.Length > 0 && nextActions[0]() is { } action)
                {
                    MaybeDispatch(action, nextActions[1..]);
                }

                _fetches.Remove(fetchName);
                StateHasChanged();
            }
        }
    }

    /// <summary>
    /// Returns true when there is a tracked <typeparamref name="FetchStartedAction"/> that is currently in a loading state.
    /// </summary>
    /// <returns></returns>
    public bool IsLoading()
    {
        foreach (string fetchName in _fetches)
        {
            if (FetchState.Value.Fetches.TryGetValue(fetchName, out var fetch) 
                && fetch is not null 
                && fetch.IsLoading 
                && !fetch.HideLoading)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Determines whether the specified fetch operation has completed successfully.
    /// </summary>
    /// <param name="fetchName">Name of the fetch operation to check.</param>
    /// <returns></returns>
    public bool IsFetchSuccessful(string fetchName)
    {
        return FetchState.Value.Fetches.TryGetValue(fetchName, out var fetch) 
            && fetch is not null 
            && !fetch.IsLoading
            && fetch.ApiError is null;
    }

    /// <summary>
    /// Update an object that needs to be stored page local state.
    /// </summary>
    /// <remarks>
    /// The object you store in page local state must be immutable.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="newLocalState"></param>
    public void UpdatePageLocalState<T>(T newLocalState)
        where T : class
    {
        var typeName = typeof(T).FullName;

        Debug.Assert(typeName is not null, "Type full name cannot be null");

        Dispatcher.Dispatch(new PageActions.UpdatePageLocalState
        {
            Name = typeName,
            LocalState = newLocalState
        });
    }

    /// <summary>
    /// Read an object is stored page local state.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="pageState"></param>
    /// <returns></returns>
    public static T ReadPageLocalState<T>(PageState pageState)
        where T : class, new()
    {
        var key = typeof(T).FullName;

        if (key is not null && pageState.PageLocalState.TryGetValue(key, out var value) && value is T tValue)
        {
            return tValue;
        }

        return new T();
    }

    protected override ValueTask DisposeAsyncCore(bool disposing)
    {
        if (disposing)
        {
            foreach (var handler in _chainedEventHandlers)
            {
                FetchState.StateChanged -= handler;
            }

            _fetches.Clear();
            _chainedEventHandlers.Clear();
        }

        return base.DisposeAsyncCore(disposing);
    }
}
