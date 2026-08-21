namespace CustomerService.Shared.Pagination
{
    public class PagedResponse<T>
    {
        public IEnumerable<T> Items { get; set; } = [];

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int TotalPages =>
            (int)Math.Ceiling(
                TotalRecords / (double)PageSize);

        public bool HasPrevious =>
            PageNumber > 1;

        public bool HasNext =>
            PageNumber < TotalPages;
    }
}
