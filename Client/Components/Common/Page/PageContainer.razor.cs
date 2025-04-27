using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using WebApp.Client.Store.PageStore;

namespace WebApp.Client.Components.Common.Page;

public partial class PageContainer : FluxorComponent
{
    /// <summary>
    /// Page heading to display
    /// </summary>
    [Parameter, EditorRequired]
    public string PageHeading { get; set; }

    /// <summary>
    /// Contents of the page.
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    [Inject]
    private IDispatcher Dispatcher { get; set; }

    protected override void OnInitialized()
    {
        Dispatcher.Dispatch(new PageActions.SetPageHeading
        {
            PageHeading = PageHeading
        });

        base.OnInitialized();
    }
}
