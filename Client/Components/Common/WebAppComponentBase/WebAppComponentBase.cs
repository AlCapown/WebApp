#nullable enable

using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using WebApp.Client.Store.FetchStore;
using WebApp.Client.Store.PageStore;
using WebApp.Client.Store.Shared;
using WebApp.Common.Models;

namespace WebApp.Client.Components.Common.WebAppComponentBase;

public abstract class WebAppComponentBase : FluxorComponent
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = default!;

    [Inject]
    private IState<FetchState> FetchState { get; set; } = default!;

    private readonly HashSet<string> _fetches;
    private readonly List<EventHandler> _chainedEventHandlers;

    protected WebAppComponentBase() : base() 
    {
        _fetches = [];
        _chainedEventHandlers = [];
    }

    /// <summary>
    /// Dispatches a <see cref="FetchStartedAction"/>. Custom middleware will dispatch the action only if 
    /// it the action as never been dispatched before or if the store cache has expired for this action.
    /// All actions are individually tracked so their loading state can be read easily using <see cref="IsLoading"/>.
    /// </summary>
    /// <param name="action">The fetch action to dispatch</param>
    /// <returns>The unique name assigned to the dispatched fetch action.</returns>
    public string MaybeDispatch(FetchStartedAction action)
    {
        string fetchName = $"{action.GetType().Name}_{action.GetHashCode()}";
        _fetches.Add(fetchName);
        Dispatcher.Dispatch(action with 
        { 
            FetchName = fetchName 
        });

        return fetchName;
    }

    /// <summary>
    /// Dispatches a <see cref="FetchStartedAction"/>. Custom middleware will dispatch the action only if 
    /// it the action as never been dispatched before or if the store cache has expired for this action.
    /// All actions are individually tracked so their loading state can be read easily using <see cref="IsLoading"/>.
    /// Actions can be chained together so that when one action completes, the next action is dispatched automatically.
    /// </summary>
    /// <param name="action">The initial fetch action to dispatch.</param>
    /// <param name="nextActions">
    /// Collection of factory functions that return the subsequent actions to dispatch in sequence. Actions are 
    /// executed in order, and each factory function is invoked only after the previous action completes.
    /// If any action factory returns null, the sequence moves to the next factory without dispatching an action.
    /// </param>
    public void MaybeDispatch(FetchStartedAction action, params Func<FetchStartedAction?>[] nextActions)
    {
        string fetchName = MaybeDispatch(action);

        if (nextActions.Length == 0)
        {
            return;
        }

        _chainedEventHandlers.Add(OnStateChangedForChainedAction);
        FetchState.StateChanged += OnStateChangedForChainedAction;

        // Trigger the check in case the fetch is already complete
        OnStateChangedForChainedAction(this, EventArgs.Empty);

        void OnStateChangedForChainedAction(object? sender, EventArgs e)
        {
            try
            {
                if (FetchState.Value.Fetches.GetValueOrDefault(fetchName) is { IsLoading: false })
                {
                    FetchState.StateChanged -= OnStateChangedForChainedAction;
                    _chainedEventHandlers.Remove(OnStateChangedForChainedAction);

                    for (int i = 0; i < nextActions.Length; i++)
                    {
                        if (nextActions[i]() is { } action)
                        {
                            MaybeDispatch(action, nextActions[(i + 1)..]);
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Dispatch(new PageActions.EnqueuePageError
                {
                    Error = new LocalError
                    {
                        TypeOfException = ex.GetType(),
                        Message = ex.Message,
                        StackTrace = ex.StackTrace
                    }
                });
            }
        }
    }

    /// <summary>
    /// Returns true when there is a tracked <typeparamref name="FetchStartedAction"/> that is currently in a loading state.
    /// </summary>
    public bool IsLoading()
    {
        foreach (string fetchName in _fetches)
        {
            if (FetchState.Value.Fetches.GetValueOrDefault(fetchName) is { IsLoading: true, HideLoading: false })
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
        return FetchState.Value.Fetches.GetValueOrDefault(fetchName) is { IsLoading: false, ApiError: null };
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

        if (key is not null && pageState.PageLocalState.GetValueOrDefault(key) is T tValue)
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
