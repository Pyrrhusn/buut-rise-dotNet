namespace Rise.Shared.Pagination
{
    public record ItemsPageDto<T>
    {
        public required IEnumerable<T> Data { get; set; }
        public int? NextId { get; set; }
        public int? PreviousId { get; set; }
        public bool IsFirstPage { get; set; }
    }

}