namespace CrmCep.Application.Common;

/// <summary>
/// Generic paginated collection result with metadata for data grid rendering.
/// </summary>
/// <typeparam name="T">Item entity type.</typeparam>
public class PaginatedResult<T>
{
    public List<T> Items { get; set; } = [];
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    public PaginatedResult() { }

    public PaginatedResult(List<T> items, int totalCount, int pageIndex, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageIndex = pageIndex;
        PageSize = pageSize;
    }
}
