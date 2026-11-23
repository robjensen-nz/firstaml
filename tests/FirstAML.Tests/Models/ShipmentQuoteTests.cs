using FirstAML.Models;

namespace FirstAML.Tests.Models;

public class ShipmentQuoteTests
{
    [Fact]
    public void QuoteIncludesIndividualCategoriesAndCostsAndTheirTotal()
    {
        var small = new PricedParcel(
            new Parcel(5, 6, 7),
            new ShippingOption(ParcelCategory.Small, 10, 3m));
        var medium = new PricedParcel(
            new Parcel(10, 20, 30),
            new ShippingOption(ParcelCategory.Medium, 50, 8m));

        var quote = new ShipmentQuote([small, medium]);

        Assert.Collection(quote.Items,
            item =>
            {
                Assert.Equal(small.Parcel, item.Parcel);
                Assert.Equal(ParcelCategory.Small, item.Category);
                Assert.Equal(3m, item.Cost);
            },
            item =>
            {
                Assert.Equal(medium.Parcel, item.Parcel);
                Assert.Equal(ParcelCategory.Medium, item.Category);
                Assert.Equal(8m, item.Cost);
            });
        Assert.Equal(11m, quote.TotalCost);
    }

    [Fact]
    public void EmptyQuoteHasZeroTotalCost()
    {
        var quote = new ShipmentQuote([]);

        Assert.Empty(quote.Items);
        Assert.Equal(0m, quote.TotalCost);
    }

    [Fact]
    public void ChangingOriginalCollectionDoesNotChangeQuote()
    {
        var item = new PricedParcel(
            new Parcel(5, 6, 7),
            new ShippingOption(ParcelCategory.Small, 10, 3m));
        var items = new List<PricedParcel> { item };
        var quote = new ShipmentQuote(items);

        items.Clear();

        Assert.Single(quote.Items);
        Assert.Equal(3m, quote.TotalCost);
    }
}
