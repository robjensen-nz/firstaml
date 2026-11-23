using FirstAML.Models;

namespace FirstAML.Tests.Models;

public class ShippingOptionTests
{
    public static TheoryData<decimal> NegativeCosts => new()
    {
        -0.01m,
        -1m,
        decimal.MinValue
    };

    public static TheoryData<decimal> ValidCosts => new()
    {
        0m,
        0.01m,
        3m,
        decimal.MaxValue
    };

    [Theory]
    [MemberData(nameof(NegativeCosts))]
    public void ConstructorRejectsNegativeCost(decimal cost)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new ShippingOption(ParcelCategory.Small, 10, cost));

        Assert.Equal("Cost", exception.ParamName);
        Assert.Equal(cost, exception.ActualValue);
    }

    [Theory]
    [MemberData(nameof(NegativeCosts))]
    public void WithExpressionRejectsNegativeCost(decimal cost)
    {
        var option = new ShippingOption(ParcelCategory.Small, 10, 3m);

        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => option with { Cost = cost });

        Assert.Equal("Cost", exception.ParamName);
        Assert.Equal(cost, exception.ActualValue);
    }

    [Theory]
    [MemberData(nameof(ValidCosts))]
    public void ConstructorAcceptsNonnegativeCost(decimal cost)
    {
        var option = new ShippingOption(ParcelCategory.Small, 10, cost);

        Assert.Equal(cost, option.Cost);
    }

    [Theory]
    [MemberData(nameof(ValidCosts))]
    public void WithExpressionAcceptsNonnegativeCost(decimal cost)
    {
        var option = new ShippingOption(ParcelCategory.Small, 10, 3m);

        var updated = option with { Cost = cost };

        Assert.Equal(cost, updated.Cost);
        Assert.Equal(3m, option.Cost);
    }
}
