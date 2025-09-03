using System;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Rise.Shared.Notifications;

namespace Rise.Client.Notifications.Components
{
    public partial class NotificationsListComponent
    {
        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object> AdditionalAttributes { get; set; } = new();
        [Parameter, EditorRequired]
        public required IEnumerable<NotificationDto> Notifications { get; set; }
        [Parameter]
        public bool Scrollbar { get; set; } = false;

        [Parameter]
        public EventCallback<NotificationDto> OnNotificationClick { get; set; }
        [Parameter]
        public NotificationDto? SelectedNotification { get; set; }
    }
}