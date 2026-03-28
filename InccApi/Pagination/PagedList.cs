using Microsoft.EntityFrameworkCore;

namespace InccApi.Pagination;

public class PagedList<T>
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public IEnumerable<T> Items { get; set; } = new List<T>();

    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling((double)count / pageSize);
    }

    public static async Task<PagedList<T>> ToPagedListAsync(IQueryable<T> source, int pageNumber, 
        int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();

        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}
