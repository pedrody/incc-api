using InccApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InccApi.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> context) : base(context)
    {
    }

    public DbSet<InccEntry>? InccEntries { get; set; }
}
