using AlejandgoShop.Domain.Catalog;
using AlejandgoShop.Domain.Enums;
using Shouldly;
using Xunit;

namespace AlejandgoShop.UnitTests.Domain;

public class ProductTests
{
    [Fact]
    public void Un_producto_nuevo_no_tiene_variantes()
    {
        var product = CreateProduct();

        product.Variants.ShouldBeEmpty();
    }

    [Fact]
    public void Se_puede_añadir_una_variante_al_producto()
    {
        var product = CreateProduct();

        product.AddVariant(color: "Negro", size: Size.M, stock: 20, sku: "CAM-NEG-M");

        product.Variants.Count.ShouldBe(1);
        product.Variants[0].Color.ShouldBe("Negro");
    }

    [Fact]
    public void No_se_puede_añadir_una_variante_con_sku_duplicado()
    {
        var product = CreateProduct();
        product.AddVariant(color: "Negro", size: Size.M, stock: 20, sku: "CAM-NEG-M");

        Should.Throw<InvalidOperationException>(() =>
            product.AddVariant(color: "Blanco", size: Size.L, stock: 5, sku: "CAM-NEG-M"));
    }

    [Fact]
    public void Un_producto_que_no_permite_personalizacion_lo_declara_correctamente()
    {
        var product = new Product(
            name: "Gorra Clásica",
            description: "Gorra ajustable",
            basePrice: 12.50m,
            category: ProductCategory.Gorra,
            allowsCustomization: false);

        product.AllowsCustomization.ShouldBeFalse();
        product.CustomDesignDimensions.ShouldBeNull();
    }

    [Fact]
    public void Un_producto_que_permite_personalizacion_debe_tener_dimensiones()
    {
        Should.Throw<ArgumentException>(() =>
            new Product(
                name: "Tapete sin dimensiones",
                description: "desc",
                basePrice: 25m,
                category: ProductCategory.Otro,
                allowsCustomization: true,
                customDesignDimensions: null));
    }

    [Fact]
    public void Un_producto_personalizable_guarda_sus_dimensiones()
    {
        var product = new Product(
            name: "Tapete TCG",
            description: "Tapete para cartas",
            basePrice: 25m,
            category: ProductCategory.Otro,
            allowsCustomization: true,
            customDesignDimensions: PrintDimensions.TcgPlaymat);

        product.CustomDesignDimensions.ShouldBe(PrintDimensions.TcgPlaymat);
    }

    private static Product CreateProduct() =>
        new(name: "Camiseta Básica",
            description: "Camiseta 100% algodón",
            basePrice: 19.99m,
            category: ProductCategory.Camiseta,
            allowsCustomization: true,
            customDesignDimensions: PrintDimensions.TcgPlaymat); // valor de prueba, no representa la realidad de negocio
}