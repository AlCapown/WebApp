#nullable enable

using Fluxor;
using System;
using System.Collections.Immutable;
using WebApp.Common.Models;

namespace WebApp.Client.Store.FetchStore;

public sealed record FetchState
{
    /// <summary>
    /// Fetch dictionary to track loading and failure state where the FetchName is the key
    /// </summary>
    public required ImmutableDictionary<string, Fetch> Fetches { get; init; }
}

public sealed record Fetch
{
    public required string FetchName { get; init; }
    public bool IsLoading { get; init; }
    public bool HideLoading { get; init; }
    public bool IsComplete { get; init; }
    public DateTimeOffset? CacheExpires { get; init; }
    public ApiError? ApiError { get; init; }
}

public sealed class FetchFeature : Feature<FetchState>
{
    public override string GetName() => "Fetches";

    protected override FetchState GetInitialState()
    {
        return new FetchState
        {
            Fetches = ImmutableDictionary.Create<string, Fetch>()
        };
    }
}

public static partial class FetchActions 
{
    public sealed record FetchStarted
    {
        public required string FetchName { get; init; }
        public bool HideLoading { get; init; }
    }

    public sealed record FetchSuccess
    {
        public required string FetchName { get; init; }
        public DateTimeOffset? CacheExpires { get; init; }
    }

    public sealed record FetchFailure
    {
        public required string FetchName { get; init; }
        public required ApiError ApiError { get; init; }
    }
}

public sealed class FetchStartedReducer : Reducer<FetchState, FetchActions.FetchStarted>
{
    public override FetchState Reduce(FetchState state, FetchActions.FetchStarted action) => state with
    {
        Fetches = state.Fetches.SetItem(action.FetchName, new Fetch
        {
            FetchName = action.FetchName,
            IsLoading = true,
            HideLoading = action.HideLoading,
            IsComplete = false,
            CacheExpires = null,
            ApiError = null
        })
    };
}

public sealed class FetchSuccessReducer : Reducer<FetchState, FetchActions.FetchSuccess>
{
    public override FetchState Reduce(FetchState state, FetchActions.FetchSuccess action) => state with
    {
        Fetches = state.Fetches.SetItem(action.FetchName, new Fetch
        {
            FetchName = action.FetchName,
            IsLoading = false,
            HideLoading = false,
            IsComplete = true,
            CacheExpires = action.CacheExpires,
            ApiError = null
        })
    };
}

public sealed class FetchFailureReducer : Reducer<FetchState, FetchActions.FetchFailure>
{
    public override FetchState Reduce(FetchState state, FetchActions.FetchFailure action) => state with
    {
        Fetches = state.Fetches.SetItem(action.FetchName, new Fetch
        {
            FetchName = action.FetchName,
            IsLoading = false,
            HideLoading = false,
            IsComplete = true,
            CacheExpires = null,
            ApiError = action.ApiError
        })
    };
}