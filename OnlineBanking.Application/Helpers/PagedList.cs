
namespace OnlineBanking.Application.Helpers;

public class PagedList<T> : List<T>
{
    private PagedList(IReadOnlyList<T> items, int count, int pageNumber, int pageSize)
    {
        CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        PageSize = pageSize;
        TotalCount = count;
        Data = items;
    }

    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public IReadOnlyList<T> Data { get; set; }

    public static PagedList<T> Create(IReadOnlyList<T> items, int totalCount, int pageNumber, int pageSize)
    {
        return new PagedList<T>(items, totalCount, pageNumber, pageSize);
    }
}