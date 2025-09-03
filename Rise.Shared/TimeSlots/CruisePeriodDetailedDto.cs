namespace Rise.Shared.TimeSlots
{
    public record CruisePeriodDetailedDto
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

    }
}
