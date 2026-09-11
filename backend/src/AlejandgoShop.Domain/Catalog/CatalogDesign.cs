using AlejandgoShop.Domain.Common;

namespace AlejandgoShop.Domain.Catalog;

public class CatalogDesign : Entity
{
    public string Name { get; private set; } = null!;
    public string ImageUrl { get; private set; } = null!;
    public bool IsPremium { get; private set; }
    public int Stock { get; private set; }

    public CatalogDesign(string name, string imageUrl, bool isPremium, int stock)
    {
        Name = name;
        ImageUrl = imageUrl;
        IsPremium = isPremium;
        Stock = stock;
    }

    private CatalogDesign() { }
}