using FirstAML.Models;

namespace FirstAML.Tests.Models;

public class ShipmentQuoteTests
{
    [Theory]
    [InlineData(1, 0)]
    [InlineData(3, 0)]
    [InlineData(3, 1)]
    [InlineData(3, 2)]
    public void ConstructorRejectsNullItemElements(int count, int nullIndex)
    {
        var item = new PricedParcel(
            new Parcel(1, 1, 1, 0), new ShippingOption(ParcelCategory.Small, 10, 3m, 1, 2m));
        var items = Enumerable.Range(0, count)
            .Select(index => index == nullIndex ? null! : item);

        var exception = Assert.Throws<ArgumentException>(
            () => new ShipmentQuote(items, ShippingSpeed.Normal));

        Assert.Equal("items", exception.ParamName);
    }

    [Fact]
    public void ConstructorRejectsNullCollection()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => new ShipmentQuote(null!, ShippingSpeed.Normal));

        Assert.Equal("items", exception.ParamName);
    }

    [Theory]
    [InlineData(-1)] // Below the defined values.
    [InlineData(2)] // Immediately above Speedy.
    [InlineData(99)] // An arbitrary undefined value.
    public void ConstructorRejectsUndefinedShippingSpeed(int value)
    {
        var shippingSpeed = (ShippingSpeed)value;

        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new ShipmentQuote([], shippingSpeed));

        Assert.Equal("shippingSpeed", exception.ParamName);
        Assert.Equal(shippingSpeed, exception.ActualValue);
    }

    [Fact]
    public void QuoteIncludesIndividualCategoriesAndCostsAndTheirTotal()
    {
        var small = new PricedParcel(
            new Parcel(5, 6, 7, 0),
            new ShippingOption(ParcelCategory.Small, 10, 3m, 1, 2m));
        var medium = new PricedParcel(
            new Parcel(10, 20, 30, 0),
            new ShippingOption(ParcelCategory.Medium, 50, 8m, 3, 2m));

        var quote = new ShipmentQuote([small, medium], ShippingSpeed.Normal);

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
        var quote = new ShipmentQuote([], ShippingSpeed.Normal);

        Assert.Empty(quote.Items);
        Assert.Equal(0m, quote.TotalCost);
    }

    [Fact]
    public void ChangingOriginalCollectionDoesNotChangeQuote()
    {
        var item = new PricedParcel(
            new Parcel(5, 6, 7, 0),
            new ShippingOption(ParcelCategory.Small, 10, 3m, 1, 2m));
        var items = new List<PricedParcel> { item };
        var quote = new ShipmentQuote(items, ShippingSpeed.Normal);

        items.Clear();

        Assert.Single(quote.Items);
        Assert.Equal(3m, quote.TotalCost);
    }
}
