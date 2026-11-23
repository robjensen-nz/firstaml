using FirstAML.Models;
using FirstAML.Services;

namespace FirstAML.Tests.Services;

public class ShippingServiceBoundaryTests
{
    public static TheoryData<int, int, int, ParcelCategory, decimal> BoundaryCases
    {
        get
        {
            (int Dimension, ParcelCategory Category, decimal Cost)[] cases =
            [
                // Just below the small/medium cutoff: still small.
                (9, ParcelCategory.Small, 3m),
                // Exactly 10 cm: the small limit is exclusive, so medium.
                (10, ParcelCategory.Medium, 8m),
                // Just above the small/medium cutoff: medium.
                (11, ParcelCategory.Medium, 8m),

                // Just below the medium/large cutoff: still medium.
                (49, ParcelCategory.Medium, 8m),
                // Exactly 50 cm: the medium limit is exclusive, so large.
                (50, ParcelCategory.Large, 15m),
                // Just above the medium/large cutoff: large.
                (51, ParcelCategory.Large, 15m),

                // Just below the large/XL cutoff: still large.
                (99, ParcelCategory.Large, 15m),
                // Exactly 100 cm: the large limit is exclusive, so XL.
                (100, ParcelCategory.XL, 25m),
                // Just above the large/XL cutoff: XL.
                (101, ParcelCategory.XL, 25m)
            ];

            var data = new TheoryData<int, int, int, ParcelCategory, decimal>();

            foreach (var testCase in cases)
            {
                // Any one dimension must be sufficient to determine the category.
                data.Add(testCase.Dimension, 1, 1, testCase.Category, testCase.Cost);
                data.Add(1, testCase.Dimension, 1, testCase.Category, testCase.Cost);
                data.Add(1, 1, testCase.Dimension, testCase.Category, testCase.Cost);
            }

            return data;
        }
    }

    [Theory]
    [MemberData(nameof(BoundaryCases))]
    public void CalculateQuoteUsesExclusiveDimensionCutoffs(
        int width, int height, int depth, ParcelCategory expectedCategory, decimal expectedCost)
    {
        var parcel = new Parcel(width, height, depth);
        var service = new ShippingService();

        var request = new ShipmentRequest([parcel], ShippingSpeed.Normal);

        var quote = service.CalculateQuote(request);

        var item = Assert.Single(quote.Items);
        Assert.Equal(parcel, item.Parcel);
        Assert.Equal(expectedCategory, item.Category);
        Assert.Equal(expectedCost, item.Cost);
    }
}
