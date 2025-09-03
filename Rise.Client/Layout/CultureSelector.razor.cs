using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System.Globalization;

namespace Rise.Client.Layout
{
    public partial class CultureSelector
    {

        /// <summary>
        /// The color of the menu, default is, Color.Secondary.
        /// </summary>
        [Parameter]
        public Color MenuColor { get; set; } = Color.Secondary;

        [Parameter]
        public bool IsMobile { get; set; }

        /// <summary>
        /// The cultures that are supported by the application.
        /// </summary> 
        private CultureInfo[] supportedCultures = new[]
        {
        new CultureInfo("nl-BE"),
        new CultureInfo("en-US"),
        };

        /// <summary>
        /// The currently selected culture. 
        /// </summary>
        private CultureInfo? selectedCulture;

        /// <summary>
        ///
        /// </summary>
        protected override void OnInitialized()
        {
            selectedCulture = CultureInfo.CurrentCulture;
        }

        /// <summary>
        ///  Handles the selection of a culture.
        /// </summary>
        private async Task OnCultureSelectedAsync(CultureInfo culture)
        {
            selectedCulture = culture;
            await ApplySelectedCultureAsync();
        }

        /// <summary>
        /// Applies the selected culture 
        /// updates the localStorage and refreshes the page.
        /// </summary>
        private async Task ApplySelectedCultureAsync()
        {
            if (CultureInfo.CurrentCulture != selectedCulture)
            {
                await JS.InvokeVoidAsync("blazorCulture.set", selectedCulture!.Name);
                Navigation.Refresh(true);
            }
        }
    }
}