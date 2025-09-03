using System;
using Rise.Shared.Pagination;

namespace Rise.Shared.Reservations;

public record ReservationDto : BaseDto
{
    public TimeOnly Start { get; set; }
    public TimeOnly End { get; set; }
    public DateOnly Date { get; set; }
    public bool IsDeleted { get; set; }
    public int BoatId { get; set; } // to later get the boat info or the boat photo
    public string BoatPersonalName { get; set; } = default!; // to show the name in own reservation list

    public string UserName { get; set; } = default!;
}
