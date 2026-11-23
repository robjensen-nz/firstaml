namespace FirstAML.Models;

/// <summary>A shipping category, its size limit, and its delivery cost.</summary>
/// <param name="Category">The parcel size category.</param>
/// <param name="MaxDimensionExclusive">
/// Every dimension must be less than this limit in centimetres.
/// Null means there is no upper limit.
/// </param>
/// <param name="Cost">The delivery cost for this option.</param>
public sealed record ShippingOption(
    ParcelCategory Category,
    int? MaxDimensionExclusive,
    decimal Cost);
