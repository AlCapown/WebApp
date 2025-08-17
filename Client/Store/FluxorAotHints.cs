using System.Diagnostics.CodeAnalysis;

namespace WebApp.Client.Store;

/* TODO THIS DOESN'T ACTUALY WORK */

/// <summary>
/// Ensures Fluxor types are preserved during AOT trimming by marking them with DynamicDependency attributes.
/// This class must be referenced somewhere in the application to be effective.
/// </summary>
public static class FluxorAotHints
{
    // Middleware
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Shared.FetchMiddleware))]
    
    // FetchStore
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(FetchStore.FetchFeature))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(FetchStore.FetchActions))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(FetchStore.FetchStartedReducer))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(FetchStore.FetchSuccessReducer))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(FetchStore.FetchFailureReducer))]

    // GamePredictionStore
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(GamePredictionStore.GamePredictionFeature))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(GamePredictionStore.GamePredictionActions))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(GamePredictionStore.CreateGamePredictionEffect))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(GamePredictionStore.CreateGamePredictionForUserEffect))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(GamePredictionStore.GamePredictionSearchEffect))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(GamePredictionStore.GamePredictionSearchSuccessReducer))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(GamePredictionStore.GetGamePredictionEffect))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(GamePredictionStore.GetGamePredictionSuccessReducer))]

    // GameStore
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(GameStore.GameFeature))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(GameStore.GameActions))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(GameStore.SearchGamesForSeasonEffect))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(GameStore.SearchGamesForSeasonSuccessReducer))]

    // PageStore
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PageStore.PageFeature))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PageStore.PageActions))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PageStore.SetPageHeadingReducer))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PageStore.UpdatePageLocalStateReducer))]

    // SeasonStore
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SeasonStore.SeasonFeature))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SeasonStore.SeasonActions))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SeasonStore.CreateSeasonEffect))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SeasonStore.LoadSeasonEffect))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SeasonStore.LoadSeasonSuccessReducer))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SeasonStore.LoadSeasonListEffect))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SeasonStore.LoadSeasonListSuccessReducer))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SeasonStore.UpdateSeasonEffect))]

    // SeasonWeekStore
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SeasonWeekStore.SeasonWeekFeature))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SeasonWeekStore.SeasonWeekActions))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SeasonWeekStore.LoadSeasonWeekEffect))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SeasonWeekStore.LoadSeasonWeekSuccessReducer))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SeasonWeekStore.LoadSeasonWeekListEffect))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SeasonWeekStore.LoadSeasonWeekListSuccessReducer))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SeasonWeekStore.UpdateSeasonWeekEffect))]

    // TeamStore
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(TeamStore.TeamFeature))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(TeamStore.TeamActions))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(TeamStore.LoadTeamsEffect))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(TeamStore.LoadTeamsSuccessReducer))]

    // UserStore
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(UserStore.UserFeature))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(UserStore.UserActions))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(UserStore.GetUserByIdEffect))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(UserStore.GetUserByIdSuccessReducer))]

    // Force instantiation to ensure attributes are processed
    static FluxorAotHints()
    {
        // This static constructor ensures the type is referenced and the attributes are processed
    }

    // Method to be called from Program.cs to ensure the type is referenced
    public static void Initialize()
    {
        // This method ensures the static constructor runs and the attributes are processed
    }
}
