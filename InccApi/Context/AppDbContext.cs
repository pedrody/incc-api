using InccApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InccApi.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> context) : base(context)
    {
    }

    public DbSet<InccEntry>? InccEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<InccEntry>(entity =>
        {
            entity.ToTable("incc_entries");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ReferenceDate)
                .IsRequired()
                .HasColumnType("date");
            entity.Property(e => e.Value)
                .IsRequired()
                .HasPrecision(18, 4);
            entity.Property(e => e.MonthlyVariation)
                .IsRequired();
        });
    }
}
