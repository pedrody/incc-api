using InccApi.Context;
using InccApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InccApi.Repositories;

public class InccRepository : IInccRepository
{
    private readonly AppDbContext _context;

    public InccRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<InccEntry>> GetAllAsync()
    {
        return await _context.InccEntries
            .OrderBy(i => i.ReferenceDate)
            .ToListAsync();
    }
}
