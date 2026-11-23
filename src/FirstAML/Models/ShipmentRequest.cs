namespace FirstAML.Models;

/// <summary>The parcels and shipping speed requested by the customer.</summary>
public sealed class ShipmentRequest
{
    public ShipmentRequest(IEnumerable<Parcel> parcels, ShippingSpeed shippingSpeed)
    {
        ArgumentNullException.ThrowIfNull(parcels);
        Parcels = parcels.ToList().AsReadOnly();
        ShippingSpeed = shippingSpeed;
    }

    public IReadOnlyList<Parcel> Parcels { get; }

    public ShippingSpeed ShippingSpeed { get; }
}
