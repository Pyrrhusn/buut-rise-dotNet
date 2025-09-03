namespace Rise.Shared.Users
{
    public record UserNameDto
    {
        public required int Id { get; set; }
        public required string FullName { get; set; }
        public required string FirstName { get; set; }
        public required string FamilyName { get; set; }
    }
}