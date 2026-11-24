using FirstAML.Models;
using FirstAML.Services;

namespace FirstAML.Tests.Services;

public class SpeedyShippingServiceTests
{
    public static IEnumerable<object[]> ItemCases => ShipmentTestData.Cases.Select(testCase =>
        new object[] { testCase.Parcels, testCase.Categories, testCase.Costs });

    public static IEnumerable<object[]> TotalCostCases => ShipmentTestData.Cases.Select(testCase =>
        new object[] { testCase.Parcels, testCase.SpeedyTotalCost });

    public static IEnumerable<object[]> SpeedyChargeCases => ShipmentTestData.Cases.Select(testCase =>
        new object[] { testCase.Parcels, testCase.TotalCost });

    [Theory]
    [MemberData(nameof(ItemCases))]
    public void CalculateQuoteReturnsExpectedItems(
        Parcel[] parcels, ParcelCategory[] expectedCategories, decimal[] expectedCosts)
    {
        var service = new ShippingService();

        var request = new ShipmentRequest(parcels, ShippingSpeed.Speedy);

        var quote = service.CalculateQuote(request);

        Assert.Equal(parcels.Length, quote.Items.Count);
        Assert.Equal(parcels, quote.Items.Select(item => item.Parcel));
        Assert.Equal(expectedCategories, quote.Items.Select(item => item.Category));
        Assert.Equal(expectedCosts, quote.Items.Select(item => item.Cost));
    }

    [Theory]
    [MemberData(nameof(TotalCostCases))]
    public void CalculateQuoteReturnsExpectedTotalCost(
        Parcel[] parcels, decimal expectedTotalCost)
    {
        var service = new ShippingService();

        var request = new ShipmentRequest(parcels, ShippingSpeed.Speedy);

        var quote = service.CalculateQuote(request);

        Assert.Equal(expectedTotalCost, quote.TotalCost);
    }

    [Fact]
    public void CalculateQuoteSelectsCheapestEligibleOptionRegardlessOfCategoryOrOrder()
    {
        // All options fit this parcel. Medium is cheapest, despite Small fitting
        // more closely and Large appearing first in the supplied options.
        var parcel = new Parcel(5, 6, 7, 0);
        var service = new ShippingService([
            new ShippingOption(ParcelCategory.Large, 100, 15m, 6, 2m),
            new ShippingOption(ParcelCategory.Small, 10, 3m, 1, 2m),
            new ShippingOption(ParcelCategory.Medium, 50, 2m, 3, 2m),
            new ShippingOption(ParcelCategory.XL, null, 25m, 10, 2m)
        ]);

        var request = new ShipmentRequest([parcel], ShippingSpeed.Speedy);

        var quote = service.CalculateQuote(request);

        var item = Assert.Single(quote.Items);
        Assert.Equal(parcel, item.Parcel);
        Assert.Equal(ParcelCategory.Medium, item.Category);
        Assert.Equal(2m, item.Cost);
    }

    [Fact]
    public void CalculateQuotePreservesRequestedShippingSpeed()
    {
        var service = new ShippingService();

        var request = new ShipmentRequest([new Parcel(5, 6, 7, 0)], ShippingSpeed.Speedy);

        var quote = service.CalculateQuote(request);

        Assert.Equal(ShippingSpeed.Speedy, quote.ShippingSpeed);
    }

    [Theory]
    [MemberData(nameof(SpeedyChargeCases))]
    public void SpeedyShippingHasASeparateCharge(Parcel[] parcels, decimal expectedCharge)
    {
        var service = new ShippingService();
        var request = new ShipmentRequest(parcels, ShippingSpeed.Speedy);

        var quote = service.CalculateQuote(request);

        var charge = Assert.Single(quote.AdditionalCharges);
        Assert.Equal(AdditionalChargeType.SpeedyShipping, charge.Type);
        Assert.Equal(expectedCharge, charge.Cost);
    }

    [Fact]
    public void SpeedyChargeUsesTheSelectedCustomRate()
    {
        var service = new ShippingService([
            new ShippingOption(ParcelCategory.Small, 10, 3m, 1, 2m),
            new ShippingOption(ParcelCategory.Medium, 50, 2m, 3, 2m)
        ]);
        var request = new ShipmentRequest([new Parcel(5, 6, 7, 0)], ShippingSpeed.Speedy);

        var quote = service.CalculateQuote(request);

        var charge = Assert.Single(quote.AdditionalCharges);
        Assert.Equal(2m, charge.Cost);
    }
}
