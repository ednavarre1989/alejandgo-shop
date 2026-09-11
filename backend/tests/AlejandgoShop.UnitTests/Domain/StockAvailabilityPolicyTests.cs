using AlejandgoShop.Domain.Catalog;
using AlejandgoShop.Domain.Enums;
using Shouldly;
using Xunit;

namespace AlejandgoShop.UnitTests.Domain;

public class StockAvailabilityPolicyTests
{
    [Fact]
    public void Sin_diseño_de_catalogo_la_disponibilidad_es_el_stock_de_la_variante()
    {
        var variant = CreateVariant(stock: 10);

        var available = StockAvailabilityPolicy.GetAvailableQuantity(variant, catalogDesign: null);

        available.ShouldBe(10);
    }

    [Fact]
    public void Con_diseño_premium_manda_el_stock_del_diseño_aunque_la_variante_tenga_mas()
    {
        var variant = CreateVariant(stock: 50);
        var design = CreateDesign(stock: 3);

        var available = StockAvailabilityPolicy.GetAvailableQuantity(variant, design);

        available.ShouldBe(3);
    }

    [Fact]
    public void Con_diseño_premium_manda_el_stock_del_diseño_aunque_sea_mayor_que_la_variante()
    {
        var variant = CreateVariant(stock: 2);
        var design = CreateDesign(stock: 100);

        var available = StockAvailabilityPolicy.GetAvailableQuantity(variant, design);

        available.ShouldBe(100);
    }

    private static ProductVariant CreateVariant(int stock) =>
        new(color: "Negro", size: Size.M, stock: stock, sku: "TEST-SKU");

    private static CatalogDesign CreateDesign(int stock) =>
        new(name: "Diseño de prueba", imageUrl: "https://example.com/design.png", isPremium: true, stock: stock);
}