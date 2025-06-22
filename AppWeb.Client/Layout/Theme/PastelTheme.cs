using MudBlazor;

namespace AppWeb.Client.Layout.Theme;

/// <summary>
/// Central pastel colour palette for the whole application.
/// Extend or adjust as the design system evolves. Dark variant placeholder provided.
/// </summary>
public static class PastelTheme
{
    public static readonly MudTheme Default = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#FF5A8A",      // pink
            Secondary = "#00AEC9",    // teal
            Info = "#00D8F9",        // cyan accent
            Background = "#FFFBE6",   // ivory / off-white
            Surface = "#FFFBE6",
            AppbarBackground = "#00AEC9",
            AppbarText = Colors.Shades.White,
            DrawerBackground = Colors.Shades.White,
            DrawerText = "#2D3748",
            DrawerIcon = "#2D3748",
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#FF5A8A",
            Info = "#00D8F9",
            Background = "#121212",
            Surface = "#1E1E1E",
            AppbarBackground = "#00AEC9",
            AppbarText = Colors.Shades.White,
            DrawerBackground = "#1E1E1E",
            DrawerText = Colors.Gray.Lighten3,
            DrawerIcon = Colors.Gray.Lighten3,
        }
    };
}