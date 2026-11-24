using FirstAML.Models;

namespace FirstAML.Tests.Models;

public class ShipmentRequestTests
{
    [Theory]
    [InlineData(1, 0)]
    [InlineData(3, 0)]
    [InlineData(3, 1)]
    [InlineData(3, 2)]
    public void ConstructorRejectsNullParcelElements(int count, int nullIndex)
    {
        var parcels = Enumerable.Range(0, count)
            .Select(index => index == nullIndex ? null! : new Parcel(1, 1, 1));

        var exception = Assert.Throws<ArgumentException>(
            () => new ShipmentRequest(parcels, ShippingSpeed.Normal));

        Assert.Equal("parcels", exception.ParamName);
    }

    [Fact]
    public void ConstructorRejectsNullCollection()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => new ShipmentRequest(null!, ShippingSpeed.Normal));

        Assert.Equal("parcels", exception.ParamName);
    }

    [Theory]
    [InlineData(-1)] // Below the defined values.
    [InlineData(2)] // Immediately above Speedy.
    [InlineData(99)] // An arbitrary undefined value.
    public void ConstructorRejectsUndefinedShippingSpeed(int value)
    {
        var shippingSpeed = (ShippingSpeed)value;

        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new ShipmentRequest([], shippingSpeed));

        Assert.Equal("shippingSpeed", exception.ParamName);
        Assert.Equal(shippingSpeed, exception.ActualValue);
    }
}
