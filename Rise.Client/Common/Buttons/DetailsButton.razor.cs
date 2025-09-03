using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Rise.Client.Common.Buttons

{
    public partial class DetailsButton : ComponentBase
    {

        [Parameter]
        public required RenderFragment ChildContent { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        [Parameter]
        public required string Href { get; set; }

        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object> AdditionalAttributes { get; set; } = new();
    }
}