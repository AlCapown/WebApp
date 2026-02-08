using MudBlazor;

namespace WebApp.Client.Common.Styles;

public static class Themes
{
    public static readonly MudTheme DarkTheme = new()
    {
        PaletteDark = new PaletteDark()
        {
            Background = "#000000",
            AppbarBackground = "#000000",
            Surface = "#000000",
            DrawerBackground = "#000000",
            Primary = "#C9DDFF",
            Secondary = "#C0BCB5"
        }
    };
}
