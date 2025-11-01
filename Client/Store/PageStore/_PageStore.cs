#nullable enable

using Fluxor;
using System.Collections.Immutable;
using WebApp.Common.Models;

namespace WebApp.Client.Store.PageStore;

public sealed record PageState
{
    public required string PageHeading { get; init; }
    public required ImmutableDictionary<string, object> PageLocalState { get; init; }
}

public sealed class PageFeature : Feature<PageState>
{
    public override string GetName() => "Page";

    protected override PageState GetInitialState() => new()
    {
        PageHeading = string.Empty,
        PageLocalState = ImmutableDictionary.Create<string, object>()
    };
}

public static partial class PageActions
{
    public sealed record SetPageHeading
    {
        public required string PageHeading { get; init; }
    }

    public sealed record EnqueuePageError
    {
        public required ErrorBase Error { get; init; }
    }

    public sealed record UpdatePageLocalState
    {
        public required string Name { get; init; }
        public object? LocalState { get; init; }
    }
}

public sealed class SetPageHeadingReducer : Reducer<PageState, PageActions.SetPageHeading>
{
    public override PageState Reduce(PageState state, PageActions.SetPageHeading action) =>
        state with
        {
            PageHeading = action.PageHeading
        };
}

public sealed class UpdatePageLocalStateReducer : Reducer<PageState, PageActions.UpdatePageLocalState>
{
    public override PageState Reduce(PageState state, PageActions.UpdatePageLocalState action) =>
        state with
        {
            PageLocalState = state.PageLocalState.SetItem(action.Name, action.LocalState)
        };
}