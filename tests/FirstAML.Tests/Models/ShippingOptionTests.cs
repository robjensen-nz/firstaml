using FirstAML.Models;

namespace FirstAML.Tests.Models;

public class ShippingOptionTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void ConstructorRejectsNonpositiveWeightLimit(int weightLimitKg)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new ShippingOption(ParcelCategory.Small, 10, 3m, weightLimitKg, 2m));

        Assert.Equal("WeightLimitKg", exception.ParamName);
        Assert.Equal(weightLimitKg, exception.ActualValue);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void WithExpressionRejectsNonpositiveWeightLimit(int weightLimitKg)
    {
        var option = new ShippingOption(ParcelCategory.Small, 10, 3m, 1, 2m);

        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => option with { WeightLimitKg = weightLimitKg });

        Assert.Equal("WeightLimitKg", exception.ParamName);
        Assert.Equal(weightLimitKg, exception.ActualValue);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public void ConstructorAndWithExpressionAcceptPositiveWeightLimit(int weightLimitKg)
    {
        var option = new ShippingOption(ParcelCategory.Small, 10, 3m, weightLimitKg, 2m);
        var updated = option with { WeightLimitKg = 3 };

        Assert.Equal(weightLimitKg, option.WeightLimitKg);
        Assert.Equal(3, updated.WeightLimitKg);
    }

    [Theory]
    [MemberData(nameof(NegativeCosts))]
    public void ConstructorRejectsNegativeOverageRate(decimal rate)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new ShippingOption(ParcelCategory.Small, 10, 3m, 1, rate));

        Assert.Equal("OveragePerKg", exception.ParamName);
        Assert.Equal(rate, exception.ActualValue);
    }

    [Theory]
    [MemberData(nameof(NegativeCosts))]
    public void WithExpressionRejectsNegativeOverageRate(decimal rate)
    {
        var option = new ShippingOption(ParcelCategory.Small, 10, 3m, 1, 2m);

        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => option with { OveragePerKg = rate });

        Assert.Equal("OveragePerKg", exception.ParamName);
        Assert.Equal(rate, exception.ActualValue);
    }

    [Theory]
    [MemberData(nameof(ValidCosts))]
    public void ConstructorAndWithExpressionAcceptNonnegativeOverageRate(decimal rate)
    {
        var option = new ShippingOption(ParcelCategory.Small, 10, 3m, 1, rate);
        var updated = option with { OveragePerKg = 0m };

        Assert.Equal(rate, option.OveragePerKg);
        Assert.Equal(0m, updated.OveragePerKg);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(4)]
    [InlineData(99)]
    public void ConstructorRejectsUndefinedCategory(int value)
    {
        var category = (ParcelCategory)value;

        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new ShippingOption(category, 10, 3m, 1, 2m));

        Assert.Equal("Category", exception.ParamName);
        Assert.Equal(category, exception.ActualValue);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(4)]
    [InlineData(99)]
    public void WithExpressionRejectsUndefinedCategory(int value)
    {
        var category = (ParcelCategory)value;
        var option = new ShippingOption(ParcelCategory.Small, 10, 3m, 1, 2m);

        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => option with { Category = category });

        Assert.Equal("Category", exception.ParamName);
        Assert.Equal(category, exception.ActualValue);
    }

    [Theory]
    [InlineData(ParcelCategory.Small)]
    [InlineData(ParcelCategory.Medium)]
    [InlineData(ParcelCategory.Large)]
    [InlineData(ParcelCategory.XL)]
    public void ConstructorAcceptsDefinedCategory(ParcelCategory category)
    {
        var option = new ShippingOption(category, 10, 3m, 1, 2m);

        Assert.Equal(category, option.Category);
    }

    [Theory]
    [InlineData(ParcelCategory.Small)]
    [InlineData(ParcelCategory.Medium)]
    [InlineData(ParcelCategory.Large)]
    [InlineData(ParcelCategory.XL)]
    public void WithExpressionAcceptsDefinedCategory(ParcelCategory category)
    {
        var option = new ShippingOption(ParcelCategory.Small, 10, 3m, 1, 2m);

        var updated = option with { Category = category };

        Assert.Equal(category, updated.Category);
        Assert.Equal(ParcelCategory.Small, option.Category);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void ConstructorRejectsNonpositiveSizeLimit(int sizeLimit)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new ShippingOption(ParcelCategory.Small, sizeLimit, 3m, 1, 2m));

        Assert.Equal("MaxDimensionExclusive", exception.ParamName);
        Assert.Equal(sizeLimit, exception.ActualValue);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void WithExpressionRejectsNonpositiveSizeLimit(int sizeLimit)
    {
        var option = new ShippingOption(ParcelCategory.Small, 10, 3m, 1, 2m);

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
        var option = new ShippingOption(ParcelCategory.Small, sizeLimit, 3m, 1, 2m);

        Assert.Equal(sizeLimit, option.MaxDimensionExclusive);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public void WithExpressionAcceptsPositiveOrUnlimitedSizeLimit(int? sizeLimit)
    {
        var option = new ShippingOption(ParcelCategory.Small, 10, 3m, 1, 2m);

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
            () => new ShippingOption(ParcelCategory.Small, 10, cost, 1, 2m));

        Assert.Equal("Cost", exception.ParamName);
        Assert.Equal(cost, exception.ActualValue);
    }

    [Theory]
    [MemberData(nameof(NegativeCosts))]
    public void WithExpressionRejectsNegativeCost(decimal cost)
    {
        var option = new ShippingOption(ParcelCategory.Small, 10, 3m, 1, 2m);

        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => option with { Cost = cost });

        Assert.Equal("Cost", exception.ParamName);
        Assert.Equal(cost, exception.ActualValue);
    }

    [Theory]
    [MemberData(nameof(ValidCosts))]
    public void ConstructorAcceptsNonnegativeCost(decimal cost)
    {
        var option = new ShippingOption(ParcelCategory.Small, 10, cost, 1, 2m);

        Assert.Equal(cost, option.Cost);
    }

    [Theory]
    [MemberData(nameof(ValidCosts))]
    public void WithExpressionAcceptsNonnegativeCost(decimal cost)
    {
        var option = new ShippingOption(ParcelCategory.Small, 10, 3m, 1, 2m);

        var updated = option with { Cost = cost };

        Assert.Equal(cost, updated.Cost);
        Assert.Equal(3m, option.Cost);
    }
}
