namespace AlejandgoShop.Domain.Catalog;

/// <summary>
/// Determina cuántas unidades hay disponibles para comprar.
/// Si hay un diseño de catálogo aplicado, su stock reemplaza al de la
/// prenda como referencia de disponibilidad (no se comparan ambos).
/// </summary>
public static class StockAvailabilityPolicy
{
    public static int GetAvailableQuantity(ProductVariant variant, CatalogDesign? catalogDesign)
    {
        return catalogDesign?.Stock ?? variant.Stock;
    }
}