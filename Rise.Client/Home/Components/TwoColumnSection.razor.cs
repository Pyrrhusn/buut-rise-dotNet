using System;
using Microsoft.AspNetCore.Components;

namespace Rise.Client.Home.Components
{
    public partial class TwoColumnSection
    {
        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object> AdditionalAttributes { get; set; } = new();
        [Parameter]
        public string ImageSrc { get; set; } = "";

        [Parameter]
        public string ImageAlt { get; set; } = "";

        [Parameter]
        public bool ImageFirst { get; set; } = true;
        [Parameter]
        public string Class { get; set; } = "";

        [Parameter, EditorRequired]
        public required RenderFragment ContentSection { get; set; }

    }
}
