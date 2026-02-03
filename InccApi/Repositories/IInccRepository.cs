using InccApi.Models;
using InccApi.Pagination;

namespace InccApi.Repositories;

public interface IInccRepository
{
    Task<(IEnumerable<InccEntry> items, int totalCount)> GetPaginatedAsync(PaginationParams paginationParams);
    Task<InccEntry?> GetByDateAsync(int year, int month);
    Task<IEnumerable<InccEntry?>> GetRangeAsync(DateTime startDate, DateTime? endDate);
}
