#nullable enable

using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Diagnostics;
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

    public WebAppComponentBase() : base() 
    {
        _fetches = [];
    }

    /// <summary>
    /// Dispatches a <typeparamref name="TAction"/> action. Custom middleware will dispatch the action only if 
    /// it the action as never been dispatched before or if the store cache has expired for this <typeparamref name="TAction"/>. 
    /// All <typeparamref name="TAction"/> are individually tracked so their loading state can be read easily 
    /// using <see cref="IsLoading"/>.
    /// </summary>
    /// <remarks>
    /// This can be only used when <typeparamref name="TAction"/> inherits from <typeparamref name="FetchStartedAction"/> since
    /// these actions have async logic with a loading state.
    /// </remarks>
    /// <typeparam name="TAction"></typeparam>
    /// <param name="action">The action to be dispatched</param>
    /// <returns>The unique fetch name that is being tracked</returns>
    public string MaybeDispatchAndTrack<TAction>(TAction action)
        where TAction : FetchStartedAction
    {
        string fetchName = Dispatcher.DispatchFetch(action);
        _fetches.Add(fetchName);
        return fetchName;
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
}
