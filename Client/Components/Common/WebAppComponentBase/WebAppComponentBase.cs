using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using WebApp.Client.Common.Extensions;
using WebApp.Client.Store.FetchStore;
using WebApp.Client.Store.PageStore;
using WebApp.Client.Store.Shared;

namespace WebApp.Client.Components.Common.WebAppComponentBase;

public class WebAppComponentBase : FluxorComponent
{
    [Inject]
    private IDispatcher Dispatcher { get; set; }

    [Inject]
    private IState<FetchState> FetchState { get; set; }

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
    public void MaybeDispatchAndTrack<TAction>(TAction action)
        where TAction : FetchStartedAction
    {
        string fetchName = Dispatcher.DispatchFetch(action);
        _fetches.Add(fetchName);
    }

    /// <summary>
    /// Returns true when there is a tracked <typeparamref name="FetchStartedAction"/> that is currently in a loading state.
    /// </summary>
    /// <returns></returns>
    public bool IsLoading()
    {
        foreach (string fetchName in _fetches)
        {
            if (FetchState.Value.Fetches.TryGetValue(fetchName, out Fetch fetch) && fetch.IsLoading && !fetch.HideLoading)
            {
                return true;
            }
        }

        return false;
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
        Dispatcher.Dispatch(new PageActions.UpdatePageLocalState
        {
            Name = typeof(T).FullName,
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
        return pageState.PageLocalState.TryGetValue(typeof(T).FullName, out var value)
            ? value as T
            : new T();
    }
}
