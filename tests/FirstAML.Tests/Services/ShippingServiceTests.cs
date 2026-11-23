using FirstAML.Models;
using FirstAML.Services;

namespace FirstAML.Tests.Services;

public class ShippingServiceTests
{
    private static readonly ShipmentCase[] Cases =
    [
        // Empty shipment: no quoted items and no charge.
        new([], [], [], 0m),

        // One small parcel: all dimensions below 10 cm.
        new([new(5, 6, 7)], [ParcelCategory.Small], [3m], 3m),

        // One medium parcel: fits below 50 cm, but not below 10 cm.
        new([new(20, 30, 40)], [ParcelCategory.Medium], [8m], 8m),

        // One large parcel: fits below 100 cm, but not below 50 cm.
        new([new(60, 70, 80)], [ParcelCategory.Large], [15m], 15m),

        // One XL parcel: at least one dimension exceeds 100 cm.
        new([new(120, 20, 30)], [ParcelCategory.XL], [25m], 25m),

        // Small and medium: each retains its own rate; $3 + $8 = $11.
        new([new(5, 6, 7), new(20, 30, 40)],
            [ParcelCategory.Small, ParcelCategory.Medium], [3m, 8m], 11m),

        // Small and large: $3 + $15 = $18.
        new([new(5, 6, 7), new(60, 70, 80)],
            [ParcelCategory.Small, ParcelCategory.Large], [3m, 15m], 18m),

        // Small and XL: $3 + $25 = $28.
        new([new(5, 6, 7), new(120, 20, 30)],
            [ParcelCategory.Small, ParcelCategory.XL], [3m, 25m], 28m),

        // Medium and large: $8 + $15 = $23.
        new([new(20, 30, 40), new(60, 70, 80)],
            [ParcelCategory.Medium, ParcelCategory.Large], [8m, 15m], 23m),

        // Medium and XL: $8 + $25 = $33.
        new([new(20, 30, 40), new(120, 20, 30)],
            [ParcelCategory.Medium, ParcelCategory.XL], [8m, 25m], 33m),

        // Large and XL: $15 + $25 = $40.
        new([new(60, 70, 80), new(120, 20, 30)],
            [ParcelCategory.Large, ParcelCategory.XL], [15m, 25m], 40m),

        // All four categories together: $3 + $8 + $15 + $25 = $51.
        new([new(5, 6, 7), new(20, 30, 40), new(60, 70, 80), new(120, 20, 30)],
            [ParcelCategory.Small, ParcelCategory.Medium, ParcelCategory.Large, ParcelCategory.XL],
            [3m, 8m, 15m, 25m], 51m),

        // 50 parcels: 13 small, 13 medium, 12 large, and 12 XL, interleaved.
        // (13 x $3) + (13 x $8) + (12 x $15) + (12 x $25) = $623.
        CreateFiftyParcelCase()
    ];

    public static IEnumerable<object[]> ItemCases => Cases.Select(testCase =>
        new object[] { testCase.Parcels, testCase.Categories, testCase.Costs });

    public static IEnumerable<object[]> TotalCostCases => Cases.Select(testCase =>
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
    public void CalculateQuoteReturnsExpectedTotalCost(Parcel[] parcels, decimal expectedTotalCost)
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
        var parcel = new Parcel(5, 6, 7);
        var service = new ShippingService([
            new ShippingOption(ParcelCategory.Large, 100, 15m),
            new ShippingOption(ParcelCategory.Small, 10, 3m),
            new ShippingOption(ParcelCategory.Medium, 50, 2m),
            new ShippingOption(ParcelCategory.XL, null, 25m)
        ]);

        var request = new ShipmentRequest([parcel], ShippingSpeed.Normal);

        var quote = service.CalculateQuote(request);

        var item = Assert.Single(quote.Items);
        Assert.Equal(parcel, item.Parcel);
        Assert.Equal(ParcelCategory.Medium, item.Category);
        Assert.Equal(2m, item.Cost);
    }

    [Theory]
    [InlineData(ShippingSpeed.Normal)]
    [InlineData(ShippingSpeed.Speedy)]
    public void CalculateQuotePreservesRequestedShippingSpeed(ShippingSpeed shippingSpeed)
    {
        var service = new ShippingService();

        var request = new ShipmentRequest([new Parcel(5, 6, 7)], shippingSpeed);

        var quote = service.CalculateQuote(request);

        Assert.Equal(shippingSpeed, quote.ShippingSpeed);
    }

    private static ShipmentCase CreateFiftyParcelCase()
    {
        (Parcel Parcel, ParcelCategory Category, decimal Cost)[] pattern =
        [
            (new(5, 6, 7), ParcelCategory.Small, 3m),
            (new(20, 30, 40), ParcelCategory.Medium, 8m),
            (new(60, 70, 80), ParcelCategory.Large, 15m),
            (new(120, 20, 30), ParcelCategory.XL, 25m)
        ];

        var items = Enumerable.Range(0, 50)
            .Select(index => pattern[index % pattern.Length])
            .ToArray();

        return new ShipmentCase(
            items.Select(item => item.Parcel).ToArray(),
            items.Select(item => item.Category).ToArray(),
            items.Select(item => item.Cost).ToArray(),
            623m);
    }

    private sealed record ShipmentCase(
        Parcel[] Parcels,
        ParcelCategory[] Categories,
        decimal[] Costs,
        decimal TotalCost);
}
