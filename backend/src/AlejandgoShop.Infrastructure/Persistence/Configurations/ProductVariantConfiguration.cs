using AlejandgoShop.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlejandgoShop.Infrastructure.Persistence.Configurations;

public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("ProductVariants");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Color)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(v => v.Sku)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(v => v.Sku)
            .IsUnique();

        builder.Property(v => v.Size)
            .HasConversion<string>()
            .HasMaxLength(10);
    }
}