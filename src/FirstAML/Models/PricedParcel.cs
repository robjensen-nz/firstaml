namespace FirstAML.Models;

public sealed record PricedParcel(Parcel Parcel, ShippingOption ShippingOption)
{
    public ParcelCategory Category => ShippingOption.Category;

    public decimal BaseCost => ShippingOption.Cost;

    public decimal ExcessWeightCharge =>
        Math.Max(0, Parcel.WeightKg - ShippingOption.WeightLimitKg) * ShippingOption.OveragePerKg;

    public decimal Cost => BaseCost + ExcessWeightCharge;
}
