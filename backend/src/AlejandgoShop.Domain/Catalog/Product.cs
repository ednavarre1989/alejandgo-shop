using AlejandgoShop.Domain.Common;
using AlejandgoShop.Domain.Enums;

namespace AlejandgoShop.Domain.Catalog;

public class Product : Entity
{
    private readonly List<ProductVariant> _variants = [];

    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public decimal BasePrice { get; private set; }
    public ProductCategory Category { get; private set; }
    public bool AllowsCustomization { get; private set; }
    public PrintDimensions? CustomDesignDimensions { get; private set; }

    public IReadOnlyList<ProductVariant> Variants => _variants.AsReadOnly();

    public Product(
        string name,
        string description,
        decimal basePrice,
        ProductCategory category,
        bool allowsCustomization,
        PrintDimensions? customDesignDimensions = null)
    {
        if (allowsCustomization && customDesignDimensions is null)
        {
            throw new ArgumentException(
                "Un producto que permite personalización debe especificar sus dimensiones de impresión.",
                nameof(customDesignDimensions));
        }

        Name = name;
        Description = description;
        BasePrice = basePrice;
        Category = category;
        AllowsCustomization = allowsCustomization;
        CustomDesignDimensions = customDesignDimensions;
    }

    private Product() { }

    public void AddVariant(string color, Size size, int stock, string sku)
    {
        if (_variants.Any(v => v.Sku == sku))
        {
            throw new InvalidOperationException($"Ya existe una variante con el SKU '{sku}' en este producto.");
        }

        _variants.Add(new ProductVariant(color, size, stock, sku));
    }
}