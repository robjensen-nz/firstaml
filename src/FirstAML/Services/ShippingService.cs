using FirstAML.Models;

namespace FirstAML.Services;

public sealed class ShippingService
{
    private readonly ShippingOption[] _shippingOptions;

    public ShippingService()
        : this([
            new ShippingOption(ParcelCategory.Small, 10, 3m),
            new ShippingOption(ParcelCategory.Medium, 50, 8m),
            new ShippingOption(ParcelCategory.Large, 100, 15m),
            new ShippingOption(ParcelCategory.XL, null, 25m)
        ])
    {
    }

    public ShippingService(IEnumerable<ShippingOption> shippingOptions)
    {
        ArgumentNullException.ThrowIfNull(shippingOptions);
        _shippingOptions = shippingOptions.ToArray();
        if (_shippingOptions.Any(option => option is null))
        {
            throw new ArgumentException(
                "Shipping options must not contain null elements.", nameof(shippingOptions));
        }

        if (_shippingOptions.Count(option => option.MaxDimensionExclusive is null) > 1)
        {
            throw new ArgumentException(
                "At most one shipping option may have no upper size limit.",
                nameof(shippingOptions));
        }
    }

    public ShipmentQuote CalculateQuote(ShipmentRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var items = new List<PricedParcel>();

        foreach (var parcel in request.Parcels)
        {
            var cheapestOption = _shippingOptions
                .Where(option => option.MaxDimensionExclusive is null
                    || parcel.MaxDimension < option.MaxDimensionExclusive.Value)
                .MinBy(option => option.Cost);

            if (cheapestOption is null)
            {
                throw new InvalidOperationException(
                    $"No shipping option is available for a parcel measuring "
                    + $"{parcel.Width} x {parcel.Height} x {parcel.Depth} cm.");
            }

            items.Add(new PricedParcel(parcel, cheapestOption));
        }

        return new ShipmentQuote(items, request.ShippingSpeed);
    }
}
