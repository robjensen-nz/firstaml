using FirstAML.Models;
using FirstAML.Services;

namespace FirstAML.Tests.Services;

public class WeightBasedShippingServiceTests
{
    public static TheoryData<int, int, ParcelCategory, decimal, decimal> WeightCases => new()
    {
        // Dimension, weight, selected category, base cost, excess-weight charge.
        { 1, 0, ParcelCategory.Small, 3m, 0m },
        { 1, 1, ParcelCategory.Small, 3m, 0m },
        { 1, 2, ParcelCategory.Small, 3m, 2m },
        { 1, 4, ParcelCategory.Small, 3m, 6m },
        { 10, 0, ParcelCategory.Medium, 8m, 0m },
        { 10, 2, ParcelCategory.Medium, 8m, 0m },
        { 10, 3, ParcelCategory.Medium, 8m, 0m },
        { 10, 4, ParcelCategory.Medium, 8m, 2m },
        { 10, 6, ParcelCategory.Medium, 8m, 6m },
        { 50, 0, ParcelCategory.Large, 15m, 0m },
        { 50, 5, ParcelCategory.Large, 15m, 0m },
        { 50, 6, ParcelCategory.Large, 15m, 0m },
        { 50, 7, ParcelCategory.Large, 15m, 2m },
        { 50, 9, ParcelCategory.Large, 15m, 6m },
        { 100, 0, ParcelCategory.XL, 25m, 0m },
        { 100, 9, ParcelCategory.XL, 25m, 0m },
        { 100, 10, ParcelCategory.XL, 25m, 0m },
        { 100, 11, ParcelCategory.XL, 25m, 2m },
        { 100, 13, ParcelCategory.XL, 25m, 6m },
        { 100, int.MaxValue, ParcelCategory.XL, 25m, 4294967274m }
    };

    [Theory]
    [MemberData(nameof(WeightCases))]
    public void DefaultOptionsApplyWeightAllowancesAndExposePriceBreakdown(
        int dimension, int weightKg, ParcelCategory category, decimal baseCost, decimal excessCharge)
    {
        var parcel = new Parcel(dimension, 1, 1, weightKg);
        var service = new ShippingService();

        var quote = service.CalculateQuote(new ShipmentRequest([parcel], ShippingSpeed.Normal));

        var item = Assert.Single(quote.Items);
        Assert.Equal(parcel, item.Parcel);
        Assert.Equal(category, item.Category);
        Assert.Equal(baseCost, item.BaseCost);
        Assert.Equal(excessCharge, item.ExcessWeightCharge);
        Assert.Equal(baseCost + excessCharge, item.Cost);
        Assert.Equal(item.Cost, quote.TotalCost);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void SelectsLargerOptionWhenItsFullPriceIsCheaperRegardlessOfOrder(bool reverse)
    {
        ShippingOption[] options =
        [
            new(ParcelCategory.Small, 10, 1m, 1, 10m),
            new(ParcelCategory.Medium, 50, 8m, 3, 1.5m),
            new(ParcelCategory.Large, 100, 15m, 6, 0m)
        ];
        var service = new ShippingService(reverse ? options.Reverse() : options);
        var request = new ShipmentRequest([new Parcel(1, 1, 1, 5)], ShippingSpeed.Normal);

        var quote = service.CalculateQuote(request);

        // Small costs $41, Medium costs $11, and Large costs $15.
        var item = Assert.Single(quote.Items);
        Assert.Equal(ParcelCategory.Medium, item.Category);
        Assert.Equal(8m, item.BaseCost);
        Assert.Equal(3m, item.ExcessWeightCharge);
        Assert.Equal(11m, item.Cost);
        Assert.Equal(11m, quote.TotalCost);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void SelectsCheaperOptionWhenAnotherEligibleOptionOverflows(bool reverse)
    {
        ShippingOption[] options =
        [
            new(ParcelCategory.Small, 10, 3m, 1, decimal.MaxValue),
            new(ParcelCategory.Medium, 50, 8m, 3, 2m)
        ];
        var service = new ShippingService(reverse ? options.Reverse() : options);
        var request = new ShipmentRequest([new Parcel(1, 1, 1, 3)], ShippingSpeed.Normal);

        var quote = service.CalculateQuote(request);

        // Small's excess charge overflows, but Medium can deliver this parcel for $8.
        var item = Assert.Single(quote.Items);
        Assert.Equal(ParcelCategory.Medium, item.Category);
        Assert.Equal(8m, item.BaseCost);
        Assert.Equal(0m, item.ExcessWeightCharge);
        Assert.Equal(8m, item.Cost);
        Assert.Equal(8m, quote.TotalCost);
    }

    [Fact]
    public void CustomWeightAllowanceCanMakeLargerOptionCheaper()
    {
        var service = new ShippingService([
            new ShippingOption(ParcelCategory.Small, 10, 3m, 1, 2m),
            new ShippingOption(ParcelCategory.Medium, 50, 8m, 10, 2m)
        ]);

        var quote = service.CalculateQuote(
            new ShipmentRequest([new Parcel(1, 1, 1, 5)], ShippingSpeed.Normal));

        // Small costs $11; Medium's custom allowance keeps its cost at $8.
        var item = Assert.Single(quote.Items);
        Assert.Equal(ParcelCategory.Medium, item.Category);
        Assert.Equal(0m, item.ExcessWeightCharge);
        Assert.Equal(8m, item.Cost);
    }

    [Fact]
    public void ZeroOverageRateAllowsOverweightParcelWithoutExtraCharge()
    {
        var service = new ShippingService([
            new ShippingOption(ParcelCategory.Small, 10, 3m, 1, 0m),
            new ShippingOption(ParcelCategory.Medium, 50, 8m, 3, 2m)
        ]);

        var quote = service.CalculateQuote(
            new ShipmentRequest([new Parcel(1, 1, 1, 100)], ShippingSpeed.Normal));

        var item = Assert.Single(quote.Items);
        Assert.Equal(ParcelCategory.Small, item.Category);
        Assert.Equal(0m, item.ExcessWeightCharge);
        Assert.Equal(3m, quote.TotalCost);
    }

    [Fact]
    public void CheaperOptionIsExcludedWhenDimensionsDoNotFit()
    {
        var service = new ShippingService([
            new ShippingOption(ParcelCategory.Small, 10, 0m, 100, 0m),
            new ShippingOption(ParcelCategory.Medium, 50, 8m, 3, 2m)
        ]);

        var quote = service.CalculateQuote(
            new ShipmentRequest([new Parcel(10, 1, 1, 5)], ShippingSpeed.Normal));

        var item = Assert.Single(quote.Items);
        Assert.Equal(ParcelCategory.Medium, item.Category);
        Assert.Equal(4m, item.ExcessWeightCharge);
        Assert.Equal(12m, quote.TotalCost);
    }

    [Theory]
    [InlineData(ShippingSpeed.Normal)]
    [InlineData(ShippingSpeed.Speedy)]
    public void MixedShipmentIncludesExcessWeightInSubtotalAndSpeedyCharge(ShippingSpeed speed)
    {
        Parcel[] parcels =
        [
            new(1, 1, 1, 4),
            new(10, 1, 1, 5),
            new(50, 1, 1, 7),
            new(100, 1, 1, 12)
        ];
        var service = new ShippingService();

        var quote = service.CalculateQuote(new ShipmentRequest(parcels, speed));

        Assert.Equal(parcels, quote.Items.Select(item => item.Parcel));
        Assert.Equal(new[] { 3m, 8m, 15m, 25m }, quote.Items.Select(item => item.BaseCost));
        Assert.Equal(new[] { 6m, 4m, 2m, 4m }, quote.Items.Select(item => item.ExcessWeightCharge));
        Assert.Equal(new[] { 9m, 12m, 17m, 29m }, quote.Items.Select(item => item.Cost));
        if (speed == ShippingSpeed.Speedy)
        {
            var charge = Assert.Single(quote.AdditionalCharges);
            Assert.Equal(AdditionalChargeType.SpeedyShipping, charge.Type);
            Assert.Equal(67m, charge.Cost);
            Assert.Equal(134m, quote.TotalCost);
        }
        else
        {
            Assert.Empty(quote.AdditionalCharges);
            Assert.Equal(67m, quote.TotalCost);
        }
    }
}
