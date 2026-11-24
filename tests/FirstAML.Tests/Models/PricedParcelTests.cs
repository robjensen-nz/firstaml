using FirstAML.Models;

namespace FirstAML.Tests.Models;

public class PricedParcelTests
{
    [Fact]
    public void PriceBreakdownReflectsUpdatedParcelAndOption()
    {
        var original = new PricedParcel(
            new Parcel(1, 1, 1, 4),
            new ShippingOption(ParcelCategory.Small, 10, 3m, 1, 2m));

        var lighter = original with { Parcel = original.Parcel with { WeightKg = 1 } };
        var cheaper = original with
        {
            ShippingOption = original.ShippingOption with { Cost = 1m, OveragePerKg = 0.5m }
        };

        Assert.Equal(3m, original.BaseCost);
        Assert.Equal(6m, original.ExcessWeightCharge);
        Assert.Equal(9m, original.Cost);
        Assert.Equal(0m, lighter.ExcessWeightCharge);
        Assert.Equal(3m, lighter.Cost);
        Assert.Equal(1m, cheaper.BaseCost);
        Assert.Equal(1.5m, cheaper.ExcessWeightCharge);
        Assert.Equal(2.5m, cheaper.Cost);
    }
}
