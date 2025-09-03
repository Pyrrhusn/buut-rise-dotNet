namespace Rise.Shared.Notifications
{
    public record NotificationDto : BaseDto
    {
        public SeverityEnum Severity { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Message { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }
}
