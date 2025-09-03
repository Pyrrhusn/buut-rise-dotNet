namespace Rise.Shared.TimeSlots
{
    public record TimeSlotDto
    {
        public int Id { get; set; }
        public TimeOnly Start { get; set; }
        public TimeOnly End { get; set; }
        public Boolean IsBookedByUser { get; set; }
    }
}
