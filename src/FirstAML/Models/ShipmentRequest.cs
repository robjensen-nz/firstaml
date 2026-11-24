namespace FirstAML.Models;

/// <summary>The parcels and shipping speed requested by the customer.</summary>
public sealed class ShipmentRequest
{
    public ShipmentRequest(IEnumerable<Parcel> parcels, ShippingSpeed shippingSpeed)
    {
        ArgumentNullException.ThrowIfNull(parcels);
        if (!Enum.IsDefined(shippingSpeed))
        {
            throw new ArgumentOutOfRangeException(
                nameof(shippingSpeed), shippingSpeed, "Unsupported shipping speed.");
        }

        Parcels = parcels.ToList().AsReadOnly();
        if (Parcels.Any(parcel => parcel is null))
        {
            throw new ArgumentException("Parcels must not contain null elements.", nameof(parcels));
        }

        ShippingSpeed = shippingSpeed;
    }

    public IReadOnlyList<Parcel> Parcels { get; }

    public ShippingSpeed ShippingSpeed { get; }
}
