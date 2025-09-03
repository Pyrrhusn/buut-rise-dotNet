using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace Rise.Client.Common.Buttons
{

    public partial class PrimaryButton : ComponentBase
    {

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object>? AdditionalAttributes { get; set; }

        [Parameter]
        public required RenderFragment ChildContent { get; set; }

        [Parameter]
        public Size Size { get; set; } = Size.Medium;


        [Parameter]
        public bool Disabled { get; set; }

    }

}