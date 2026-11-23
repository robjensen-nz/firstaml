namespace FirstAML.Models;

public sealed class ShipmentQuote
{
    public ShipmentQuote(IEnumerable<PricedParcel> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        Items = items.ToList().AsReadOnly();
    }

    public IReadOnlyList<PricedParcel> Items { get; }

    public decimal TotalCost => Items.Sum(item => item.Cost);
}
