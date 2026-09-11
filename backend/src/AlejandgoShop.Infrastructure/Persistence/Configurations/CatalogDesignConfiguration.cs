using AlejandgoShop.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlejandgoShop.Infrastructure.Persistence.Configurations;

public class CatalogDesignConfiguration : IEntityTypeConfiguration<CatalogDesign>
{
    public void Configure(EntityTypeBuilder<CatalogDesign> builder)
    {
        builder.ToTable("CatalogDesigns");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.ImageUrl)
            .IsRequired()
            .HasMaxLength(500);
    }
}