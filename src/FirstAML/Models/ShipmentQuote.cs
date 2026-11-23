namespace FirstAML.Models;

public sealed class ShipmentQuote
{
    public ShipmentQuote(IEnumerable<PricedParcel> items, ShippingSpeed shippingSpeed)
    {
        ArgumentNullException.ThrowIfNull(items);
        if (!Enum.IsDefined(shippingSpeed))
        {
            throw new ArgumentOutOfRangeException(
                nameof(shippingSpeed), shippingSpeed, "Unsupported shipping speed.");
        }

        Items = items.ToList().AsReadOnly();
        ShippingSpeed = shippingSpeed;
        var parcelSubtotal = Items.Sum(item => item.Cost);
        var additionalCharges = new List<ShippingCharge>();

        if (shippingSpeed == ShippingSpeed.Speedy)
        {
            additionalCharges.Add(new ShippingCharge(AdditionalChargeType.SpeedyShipping, parcelSubtotal));
        }

        AdditionalCharges = additionalCharges.AsReadOnly();
        TotalCost = parcelSubtotal + AdditionalCharges.Sum(charge => charge.Cost);
    }

    public IReadOnlyList<PricedParcel> Items { get; }

    public ShippingSpeed ShippingSpeed { get; }

    public IReadOnlyList<ShippingCharge> AdditionalCharges { get; }

    public decimal TotalCost { get; }
}
