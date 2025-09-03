using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Rise.Shared.Notifications;

namespace Rise.Client.Notifications.Components
{
    public partial class NotificationComponent
    {
        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object> AdditionalAttributes { get; set; } = new();

        [Parameter, EditorRequired]
        public required NotificationDto Notification { get; set; }
        [Parameter]
        public string Class { get; set; } = "";
        [Parameter]
        public EventCallback OnClick { get; set; }

        private async Task HandleClick()
        {
            await OnClick.InvokeAsync();
        }
    }
}