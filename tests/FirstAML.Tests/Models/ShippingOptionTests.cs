using FirstAML.Models;

namespace FirstAML.Tests.Models;

public class ShippingOptionTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void ConstructorRejectsNonpositiveSizeLimit(int sizeLimit)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new ShippingOption(ParcelCategory.Small, sizeLimit, 3m));

        Assert.Equal("MaxDimensionExclusive", exception.ParamName);
        Assert.Equal(sizeLimit, exception.ActualValue);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void WithExpressionRejectsNonpositiveSizeLimit(int sizeLimit)
    {
        var option = new ShippingOption(ParcelCategory.Small, 10, 3m);

        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => option with { MaxDimensionExclusive = sizeLimit });

        Assert.Equal("MaxDimensionExclusive", exception.ParamName);
        Assert.Equal(sizeLimit, exception.ActualValue);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public void ConstructorAcceptsPositiveOrUnlimitedSizeLimit(int? sizeLimit)
    {
        var option = new ShippingOption(ParcelCategory.Small, sizeLimit, 3m);

        Assert.Equal(sizeLimit, option.MaxDimensionExclusive);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public void WithExpressionAcceptsPositiveOrUnlimitedSizeLimit(int? sizeLimit)
    {
        var option = new ShippingOption(ParcelCategory.Small, 10, 3m);

        var updated = option with { MaxDimensionExclusive = sizeLimit };

        Assert.Equal(sizeLimit, updated.MaxDimensionExclusive);
        Assert.Equal(10, option.MaxDimensionExclusive);
    }

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
