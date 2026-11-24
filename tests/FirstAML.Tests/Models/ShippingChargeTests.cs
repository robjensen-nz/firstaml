using FirstAML.Models;

namespace FirstAML.Tests.Models;

public class ShippingChargeTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    [InlineData(99)]
    public void ConstructorRejectsUndefinedType(int value)
    {
        var type = (AdditionalChargeType)value;

        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new ShippingCharge(type, 3m));

        Assert.Equal("Type", exception.ParamName);
        Assert.Equal(type, exception.ActualValue);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    [InlineData(99)]
    public void WithExpressionRejectsUndefinedType(int value)
    {
        var type = (AdditionalChargeType)value;
        var charge = new ShippingCharge(AdditionalChargeType.SpeedyShipping, 3m);

        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => charge with { Type = type });

        Assert.Equal("Type", exception.ParamName);
        Assert.Equal(type, exception.ActualValue);
    }

    [Fact]
    public void ConstructorAcceptsDefinedType()
    {
        var charge = new ShippingCharge(AdditionalChargeType.SpeedyShipping, 3m);

        Assert.Equal(AdditionalChargeType.SpeedyShipping, charge.Type);
        Assert.Equal(3m, charge.Cost);
    }

    [Fact]
    public void WithExpressionAcceptsDefinedType()
    {
        var charge = new ShippingCharge(AdditionalChargeType.SpeedyShipping, 3m);

        var updated = charge with { Type = AdditionalChargeType.SpeedyShipping };

        Assert.Equal(AdditionalChargeType.SpeedyShipping, updated.Type);
        Assert.Equal(3m, updated.Cost);
    }
}
