namespace InccApi.Pagination;

public class PaginationParams
{
    private const int MaxPageSize = 60;
    private const int DefaultPageSize = 12;
    
    public int PageNumber { get; set; } = 1;

    private int _pageSize = DefaultPageSize; 
    
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value <= 0) ? DefaultPageSize : (value > MaxPageSize ? MaxPageSize : value);
    }
}
