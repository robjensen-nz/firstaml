namespace FirstAML.Models;

public sealed class ShipmentQuote
{
    public ShipmentQuote(IEnumerable<PricedParcel> items, ShippingSpeed shippingSpeed)
    {
        ArgumentNullException.ThrowIfNull(items);
        Items = items.ToList().AsReadOnly();
        ShippingSpeed = shippingSpeed;
        TotalCost = Items.Sum(item => item.Cost);
    }

    public IReadOnlyList<PricedParcel> Items { get; }

    public ShippingSpeed ShippingSpeed { get; }

    public decimal TotalCost { get; }
}
