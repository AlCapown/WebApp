#nullable enable

using System;

namespace WebApp.Client.Store.Shared;

/// <summary>
/// Configuration flags for controlling fetch operation behavior.
/// </summary>
[Flags]
public enum FetchOptions
{
    /// <summary>
    /// No special options
    /// </summary>
    None = 0,
    
    /// <summary>
    /// Suppresses loading indicators in the UI during the fetch operation.
    /// </summary>
    HideLoading = 1,

    /// <summary>
    /// Routes fetch errors to the global error display system.
    /// When enabled, errors appear as user visible notifications.
    /// When disabled, errors errors will not be shown to the user.
    /// </summary>
    DispatchErrorToWindow = 2,
    
    /// <summary>
    /// Bypasses cache checks and executes the fetch immediately.
    /// </summary>
    ForceDispatch = 4,

    #region Convenient Combinations
    
    /// <summary>
    /// Standard fetch behavior: respects cache, no loading indicators, displays errors globally.
    /// </summary>
    Default = DispatchErrorToWindow,

    /// <summary>
    /// Background data refresh: bypasses cache, hidden loading, silent error handling.
    /// </summary>
    SilentRefresh = HideLoading | ForceDispatch,
    
    /// <summary>
    /// Form submission behavior: bypasses cache to ensure execution and shows loading.
    /// </summary>
    FormSubmit = ForceDispatch,

    #endregion
}
