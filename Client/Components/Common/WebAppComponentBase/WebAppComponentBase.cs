using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
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

    /// <summary>
    /// Represents the default timeout interval for sequential fetch operations.
    /// </summary>
    /// <remarks>
    /// This is strictly here to prevent deadlocks in the case of chained fetches 
    /// where one fetch depends on the completion of another fetch that, due to a bug, 
    /// never completes. 
    /// </remarks>
    private static readonly TimeSpan DefaultSequentialFetchTimeout = TimeSpan.FromSeconds(60);

    private readonly HashSet<string> _fetches;
    private readonly HashSet<string> _chainedLoadingFetches;

    protected WebAppComponentBase() : base()
    {
        _fetches = [];
        _chainedLoadingFetches = [];
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
        var fetchName = action.GenerateFetchName();
        _fetches.Add(fetchName);
        Dispatcher.Dispatch(action with
        {
            FetchName = fetchName,
        });
        return fetchName;
    }

    /// <summary>
    /// <para>
    /// Dispatches a <see cref="FetchStartedAction"/> and awaits each chained action in sequence.
    /// Custom middleware will dispatch the action only if it has never been dispatched before or
    /// if the store cache has expired for this action.
    /// </para>
    /// <para>
    /// All actions are individually tracked so their loading state can be read easily using <see cref="IsLoading"/>.
    /// Actions are executed in order; each factory is invoked only after the previous action completes.
    /// If any factory returns null, the sequence moves to the next factory without dispatching an action.
    /// </para>
    /// </summary>
    /// <param name="action">The initial fetch action to dispatch.</param>
    /// <param name="nextActions">
    /// Collection of factory functions that return the subsequent actions to dispatch in sequence.
    /// </param>
    public void MaybeDispatch(FetchStartedAction action, params Func<FetchStartedAction?>[] nextActions)
    {
        // Intentional fire-and-forget.
        // Errors are caught and dispatched to the page store
        _ = InternalMaybeDispatchAsync(action, nextActions, chainRootFetchName: null);
    }

    private async Task InternalMaybeDispatchAsync(FetchStartedAction action, Func<FetchStartedAction?>[] nextActions, string? chainRootFetchName)
    {
        string fetchName = MaybeDispatch(action);

        // Only the root call owns the _chainedFetches entry for the entire chain.
        // This is to prevent UI flickering due to intermediate fetches completing while the chain is still in progress.
        // chainRootFetchName is set on the first dispatch that shows loading.
        bool isChainRoot = chainRootFetchName is null;

        if (isChainRoot && !action.FetchOptions.HasFlag(FetchOptions.HideLoading))
        {
            chainRootFetchName = fetchName;
            _chainedLoadingFetches.Add(fetchName);
        }

        try
        {
            await WaitForFetchCompletedAsync(fetchName, DefaultSequentialFetchTimeout);

            for (int i = 0; i < nextActions.Length; i++)
            {
                if (nextActions[i]() is { } nextAction)
                {
                    // Pass the root fetch name down so recursive calls don't touch _chainedFetches
                    await InternalMaybeDispatchAsync(nextAction, nextActions[(i + 1)..], chainRootFetchName);
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Dispatcher.Dispatch(new PageActions.EnqueuePageError
            {
                Error = new LocalError
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                }
            });
        }
        finally
        {
            // Only the root removes itself after the entire chain has completed
            if (isChainRoot && chainRootFetchName is not null)
            {
                _chainedLoadingFetches.Remove(chainRootFetchName);
            }

            StateHasChanged();
        }
    }

    /// <summary>
    /// Returns a <see cref="Task"/> that completes when the given fetch is no longer loading, or
    /// throws <see cref="TimeoutException"/> if the fetch does not complete within the specified timeout.
    /// </summary>
    private Task WaitForFetchCompletedAsync(string fetchName, TimeSpan timeout)
    {
        if (FetchState.Value.Fetches.GetValueOrDefault(fetchName) is not { IsLoading: true })
        {
            return Task.CompletedTask;
        }

        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var cts = new CancellationTokenSource(timeout);

        cts.Token.Register(() =>
        {
            FetchState.StateChanged -= OnStateChanged;
            cts.Dispose();
            tcs.TrySetException(new TimeoutException($"Fetch timed out."));
        });

        FetchState.StateChanged += OnStateChanged;

        // Double-check after subscribing to check if state could have already changed.
        if (FetchState.Value.Fetches.GetValueOrDefault(fetchName) is not { IsLoading: true })
        {
            FetchState.StateChanged -= OnStateChanged;
            cts.Dispose();
            tcs.TrySetResult();
        }

        return tcs.Task;

        void OnStateChanged(object? sender, EventArgs e)
        {
            if (FetchState.Value.Fetches.GetValueOrDefault(fetchName) is not { IsLoading: true })
            {
                FetchState.StateChanged -= OnStateChanged;
                cts.Dispose();
                tcs.TrySetResult();
            }
        }
    }

    /// <summary>
    /// Returns true when there is a tracked <typeparamref name="FetchStartedAction"/> that is currently in a loading state.
    /// </summary>
    public bool IsLoading()
    {
        if (_chainedLoadingFetches.Count > 0)
        {
            return true;
        }
        
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
            _fetches.Clear();
            _chainedLoadingFetches.Clear();
        }

        return base.DisposeAsyncCore(disposing);
    }
}
