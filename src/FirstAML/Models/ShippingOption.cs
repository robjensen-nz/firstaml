namespace FirstAML.Models;

/// <summary>A shipping category, its size limit, and its delivery cost.</summary>
/// <param name="Category">The parcel size category.</param>
/// <param name="MaxDimensionExclusive">
/// Every dimension must be less than this limit in centimetres.
/// Must be positive when specified. Null means there is no upper limit.
/// </param>
/// <param name="Cost">The nonnegative delivery cost for this option.</param>
public sealed record ShippingOption(
    ParcelCategory Category,
    int? MaxDimensionExclusive,
    decimal Cost)
{
    private int? _maxDimensionExclusive = ValidateSizeLimit(MaxDimensionExclusive);
    private decimal _cost = ValidateCost(Cost);

    public int? MaxDimensionExclusive
    {
        get => _maxDimensionExclusive;
        init => _maxDimensionExclusive = ValidateSizeLimit(value);
    }

    public decimal Cost
    {
        get => _cost;
        init => _cost = ValidateCost(value);
    }

    private static int? ValidateSizeLimit(int? sizeLimit)
    {
        if (sizeLimit is <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(MaxDimensionExclusive), sizeLimit,
                "Shipping option size limit must be positive when specified.");
        }

        return sizeLimit;
    }

    private static decimal ValidateCost(decimal cost)
    {
        if (cost < 0m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(Cost), cost, "Shipping option cost must not be negative.");
        }

        return cost;
    }
}
