namespace Rise.Shared.Reservations;

public record ReservationDetailsDto : BaseDto
{
    public TimeOnly Start { get; set; }
    public TimeOnly End { get; set; }
    public DateOnly Date { get; set; }
    public int BoatId { get; set; }
    public string BoatPersonalName { get; set; } = default!;

    public string? MentorName { get; set; }

    public bool IsDeleted { get; set; }

    public string? BatteryType { get; set; }
    public int? BatteryId { get; set; }
    public string? CurrentBatteryUserName { get; set; }
    public int? CurrentBatteryUserId { get; set; }
    
    // New properties for current holder contact details
    public string? CurrentHolderPhoneNumber { get; set; }
    public string? CurrentHolderEmail { get; set; }
    public string? CurrentHolderStreet { get; set; }
    public string? CurrentHolderNumber { get; set; }
    public string? CurrentHolderCity { get; set; }
    public string? CurrentHolderPostalCode { get; set; }
}
