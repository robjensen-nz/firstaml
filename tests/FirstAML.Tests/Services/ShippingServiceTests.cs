using FirstAML.Models;
using FirstAML.Services;

namespace FirstAML.Tests.Services;

public class ShippingServiceTests
{
    [Theory]
    [InlineData(1, 0)]
    [InlineData(3, 0)]
    [InlineData(3, 1)]
    [InlineData(3, 2)]
    public void ConstructorRejectsNullOptionElements(int count, int nullIndex)
    {
        var options = Enumerable.Range(0, count)
            .Select(index => index == nullIndex
                ? null!
                : new ShippingOption(ParcelCategory.Small, 10, 3m, 1, 2m));

        var exception = Assert.Throws<ArgumentException>(() => new ShippingService(options));

        Assert.Equal("shippingOptions", exception.ParamName);
    }

    [Fact]
    public void ConstructorRejectsNullCollection()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new ShippingService(null!));

        Assert.Equal("shippingOptions", exception.ParamName);
    }

    [Fact]
    public void EmptyOptionsCanQuoteEmptyShipment()
    {
        var service = new ShippingService([]);

        var quote = service.CalculateQuote(new ShipmentRequest([], ShippingSpeed.Normal));

        Assert.Empty(quote.Items);
        Assert.Equal(0m, quote.TotalCost);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    public void ConstructorRejectsMultipleUnlimitedOptions(int count)
    {
        var options = Enumerable.Range(0, count)
            .Select(index => new ShippingOption(ParcelCategory.XL, null, 25m + index, 10, 2m));

        var exception = Assert.Throws<ArgumentException>(() => new ShippingService(options));

        Assert.Equal("shippingOptions", exception.ParamName);
    }

    [Fact]
    public void SingleUnlimitedOptionCanPriceOversizedParcel()
    {
        var service = new ShippingService([
            new ShippingOption(ParcelCategory.Small, 10, 3m, 1, 2m),
            new ShippingOption(ParcelCategory.XL, null, 25m, 10, 2m)
        ]);
        var request = new ShipmentRequest([new Parcel(int.MaxValue, 1, 1, 0)], ShippingSpeed.Normal);

        var quote = service.CalculateQuote(request);

        Assert.Equal(ParcelCategory.XL, Assert.Single(quote.Items).Category);
        Assert.Equal(25m, quote.TotalCost);
    }

    [Fact]
    public void OptionsWithoutUnlimitedSizeCanPriceEligibleParcel()
    {
        var service = new ShippingService([new ShippingOption(ParcelCategory.Small, 1, 3m, 1, 2m)]);
        var request = new ShipmentRequest([new Parcel(0, 0, 0, 0)], ShippingSpeed.Normal);

        var quote = service.CalculateQuote(request);

        Assert.Equal(ParcelCategory.Small, Assert.Single(quote.Items).Category);
        Assert.Equal(3m, quote.TotalCost);
    }

    [Fact]
    public void OptionsWithoutUnlimitedSizeRejectParcelAtUpperLimit()
    {
        var service = new ShippingService([new ShippingOption(ParcelCategory.Small, 10, 3m, 1, 2m)]);
        var request = new ShipmentRequest([new Parcel(10, 1, 1, 0)], ShippingSpeed.Normal);

        Assert.Throws<InvalidOperationException>(() => service.CalculateQuote(request));
    }

    public static IEnumerable<object[]> ItemCases => ShipmentTestData.Cases.Select(testCase =>
        new object[] { testCase.Parcels, testCase.Categories, testCase.Costs });

    public static IEnumerable<object[]> TotalCostCases => ShipmentTestData.Cases.Select(testCase =>
        new object[] { testCase.Parcels, testCase.TotalCost });

    [Theory]
    [MemberData(nameof(ItemCases))]
    public void CalculateQuoteReturnsExpectedItems(
        Parcel[] parcels, ParcelCategory[] expectedCategories, decimal[] expectedCosts)
    {
        var service = new ShippingService();

        var request = new ShipmentRequest(parcels, ShippingSpeed.Normal);

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

        var request = new ShipmentRequest(parcels, ShippingSpeed.Normal);

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

        var request = new ShipmentRequest([parcel], ShippingSpeed.Normal);

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

        var request = new ShipmentRequest([new Parcel(5, 6, 7, 0)], ShippingSpeed.Normal);

        var quote = service.CalculateQuote(request);

        Assert.Equal(ShippingSpeed.Normal, quote.ShippingSpeed);
    }

    [Theory]
    [InlineData(ShippingSpeed.Normal)]
    [InlineData(ShippingSpeed.Speedy)]
    public void CalculateQuoteSelectsFreeEligibleOption(ShippingSpeed shippingSpeed)
    {
        var service = new ShippingService([
            new ShippingOption(ParcelCategory.Small, 10, 3m, 1, 2m),
            new ShippingOption(ParcelCategory.Medium, 50, 0m, 3, 2m)
        ]);
        var request = new ShipmentRequest([new Parcel(5, 6, 7, 0)], shippingSpeed);

        var quote = service.CalculateQuote(request);

        var item = Assert.Single(quote.Items);
        Assert.Equal(ParcelCategory.Medium, item.Category);
        Assert.Equal(0m, item.Cost);
        Assert.Equal(0m, quote.TotalCost);
        if (shippingSpeed == ShippingSpeed.Speedy)
        {
            Assert.Equal(0m, Assert.Single(quote.AdditionalCharges).Cost);
        }
        else
        {
            Assert.Empty(quote.AdditionalCharges);
        }
    }

    [Fact]
    public void NormalShippingHasNoAdditionalCharge()
    {
        var service = new ShippingService();
        var request = new ShipmentRequest([new Parcel(5, 6, 7, 0)], ShippingSpeed.Normal);

        var quote = service.CalculateQuote(request);

        Assert.Empty(quote.AdditionalCharges);
    }
}
