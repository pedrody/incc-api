using InccApi.Models;
using InccApi.Pagination;

namespace InccApi.Repositories;

public interface IInccRepository
{
    Task<PagedList<InccEntry>> GetPaginatedAsync(PaginationParams paginationParams);
    Task<InccEntry?> GetByDateAsync(int year, int month);
    Task<PagedList<InccEntry>> GetRangeAsync(PaginationParams @params, 
                                             DateTime startDate, DateTime? endDate);
}
