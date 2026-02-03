namespace InccApi.Pagination;

public class PaginationParams
{
    private const int MaxPageSize = 60;
    public int PageNumber { get; set; } = 1;
    public int PageSize
    {
        get => field;
        set => field = (value > MaxPageSize) ? MaxPageSize : value;
    }
}
