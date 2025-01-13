

namespace OnlineBanking.Core.Helpers;

public class PagedList<T> : List<T>
{
    public PagedList(IReadOnlyList<T> items, int count, int pageNumber, int pageSize)
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
        items = items.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new PagedList<T>(items, totalCount, pageNumber, pageSize);
    }
}