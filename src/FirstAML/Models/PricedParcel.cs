namespace FirstAML.Models;

public sealed record PricedParcel(Parcel Parcel, ShippingOption ShippingOption)
{
    public ParcelCategory Category => ShippingOption.Category;

    public decimal Cost => ShippingOption.Cost;
}
