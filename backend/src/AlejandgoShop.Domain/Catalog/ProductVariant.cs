using AlejandgoShop.Domain.Common;
using AlejandgoShop.Domain.Enums;

namespace AlejandgoShop.Domain.Catalog;

public class ProductVariant : Entity
{
    public string Color { get; private set; } = null!;
    public Size Size { get; private set; }
    public int Stock { get; private set; }
    public string Sku { get; private set; } = null!;

    public ProductVariant(string color, Size size, int stock, string sku)
    {
        Color = color;
        Size = size;
        Stock = stock;
        Sku = sku;
    }

    // EF Core necesita un constructor sin parámetros (o privado) para materializar entidades
    private ProductVariant() { }
}