using AlejandgoShop.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlejandgoShop.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(p => p.BasePrice)
            .HasPrecision(10, 2);

        builder.Property(p => p.Category)
            .HasConversion<string>()
            .HasMaxLength(20);

        // Product es el agregado raíz: sus variantes viven "dentro" de él.
        // EF Core necesita acceso al campo privado _variants para materializar la colección.
        builder.Metadata.FindNavigation(nameof(Product.Variants))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(p => p.Variants)
            .WithOne()
            .HasForeignKey("ProductId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}