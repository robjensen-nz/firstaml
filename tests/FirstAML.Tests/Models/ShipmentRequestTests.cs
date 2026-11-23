using FirstAML.Models;

namespace FirstAML.Tests.Models;

public class ShipmentRequestTests
{
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
