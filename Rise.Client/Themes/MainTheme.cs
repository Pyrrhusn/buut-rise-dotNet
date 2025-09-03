using MudBlazor;

namespace Rise.Client.Themes
{
    public static class MainTheme
    {
        public static MudTheme CustomTheme = new()
        {
            PaletteLight = new PaletteLight()
            {
                Primary = "#42C4BE",
                PrimaryLighten = "#65D0CB",
                PrimaryDarken = "#339995",
                Secondary = "#ffffff",
                SecondaryContrastText = "#272c34",
                AppbarBackground = "#42C4BE",
            },
            PaletteDark = new PaletteDark()
            {
                Primary = "#42C4BE",
            },
            LayoutProperties = new LayoutProperties()
            {
                DrawerWidthLeft = "260px",
                DrawerWidthRight = "300px"
            }
        };
    }
}