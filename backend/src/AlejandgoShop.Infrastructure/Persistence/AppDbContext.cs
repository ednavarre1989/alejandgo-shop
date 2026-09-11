using AlejandgoShop.Domain.Catalog;
using Microsoft.EntityFrameworkCore;

namespace AlejandgoShop.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<CatalogDesign> CatalogDesigns => Set<CatalogDesign>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}