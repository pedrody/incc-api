using InccApi.Context;
using InccApi.Models;
using InccApi.Pagination;
using Microsoft.EntityFrameworkCore;

namespace InccApi.Repositories;

public class InccRepository : IInccRepository
{
    private readonly AppDbContext _context;

    public InccRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedList<InccEntry>> GetPaginatedAsync(PaginationParams paginationParams)
    {
        var query = _context.InccEntries.AsNoTracking().OrderBy(e => e.ReferenceDate);

        return await PagedList<InccEntry>.ToPagedListAsync(query, paginationParams.PageNumber, paginationParams.PageSize);
    }

    public async Task<InccEntry?> GetByDateAsync(int year, int month)
    {
        var date = new DateTime(year, month, 1);

        return await _context.InccEntries.FirstOrDefaultAsync(e =>
            e.ReferenceDate == date
        );
    }

    public async Task<PagedList<InccEntry>> GetRangeAsync(
        PaginationParams @params,
        DateTime startDate, DateTime? endDate)
    {
        var query = _context.InccEntries.Where(e => e.ReferenceDate >= startDate);

        if (endDate.HasValue)
        {
            query = query.Where(e => e.ReferenceDate <= endDate.Value);
        }

        return await PagedList<InccEntry>.ToPagedListAsync(query.OrderBy(e => e.ReferenceDate), 
                                                           @params.PageNumber, @params.PageSize);
    }
}
