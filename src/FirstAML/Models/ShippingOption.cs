namespace FirstAML.Models;

/// <summary>A shipping category, its size limit, and its delivery cost.</summary>
/// <param name="Category">The parcel size category.</param>
/// <param name="MaxDimensionExclusive">
/// Every dimension must be less than this limit in centimetres.
/// Must be positive when specified. Null means there is no upper limit.
/// </param>
/// <param name="Cost">The nonnegative base delivery cost for this option.</param>
/// <param name="WeightLimitKg">The positive whole-kilogram weight allowance.</param>
/// <param name="OveragePerKg">The nonnegative charge per kilogram above the allowance.</param>
public sealed record ShippingOption(
    ParcelCategory Category,
    int? MaxDimensionExclusive,
    decimal Cost,
    int WeightLimitKg,
    decimal OveragePerKg)
{
    private ParcelCategory _category = ValidateCategory(Category);
    private int? _maxDimensionExclusive = ValidateSizeLimit(MaxDimensionExclusive);
    private decimal _cost = ValidateCost(Cost);
    private int _weightLimitKg = ValidateWeightLimit(WeightLimitKg);
    private decimal _overagePerKg = ValidateOverage(OveragePerKg);

    public int WeightLimitKg
    {
        get => _weightLimitKg;
        init => _weightLimitKg = ValidateWeightLimit(value);
    }

    public decimal OveragePerKg
    {
        get => _overagePerKg;
        init => _overagePerKg = ValidateOverage(value);
    }

    public ParcelCategory Category
    {
        get => _category;
        init => _category = ValidateCategory(value);
    }

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

    private static ParcelCategory ValidateCategory(ParcelCategory category)
    {
        if (!Enum.IsDefined(category))
        {
            throw new ArgumentOutOfRangeException(
                nameof(Category), category, "Unsupported parcel category.");
        }

        return category;
    }

    private static int ValidateWeightLimit(int weightLimitKg)
    {
        if (weightLimitKg <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(WeightLimitKg), weightLimitKg, "Weight limit must be positive.");
        }

        return weightLimitKg;
    }

    private static decimal ValidateOverage(decimal overagePerKg)
    {
        if (overagePerKg < 0m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(OveragePerKg), overagePerKg, "Overage rate must not be negative.");
        }

        return overagePerKg;
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
