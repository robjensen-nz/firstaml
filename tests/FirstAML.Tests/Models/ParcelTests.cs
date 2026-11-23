using FirstAML.Models;

namespace FirstAML.Tests.Models;

public class ParcelTests
{
    [Theory]
    [InlineData(-1, 1, 1, "Width", -1)]
    [InlineData(1, -1, 1, "Height", -1)]
    [InlineData(1, 1, -1, "Depth", -1)]
    [InlineData(int.MinValue, 1, 1, "Width", int.MinValue)]
    [InlineData(1, int.MinValue, 1, "Height", int.MinValue)]
    [InlineData(1, 1, int.MinValue, "Depth", int.MinValue)]
    public void ConstructorRejectsNegativeDimensions(
        int width, int height, int depth, string parameterName, int invalidValue)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new Parcel(width, height, depth));

        Assert.Equal(parameterName, exception.ParamName);
        Assert.Equal(invalidValue, exception.ActualValue);
    }

    [Theory]
    [InlineData("Width")]
    [InlineData("Height")]
    [InlineData("Depth")]
    public void WithExpressionRejectsNegativeDimensions(string dimension)
    {
        var parcel = new Parcel(1, 1, 1);

        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => dimension switch
        {
            "Width" => parcel with { Width = -1 },
            "Height" => parcel with { Height = -1 },
            _ => parcel with { Depth = -1 }
        });

        Assert.Equal(dimension, exception.ParamName);
        Assert.Equal(-1, exception.ActualValue);
    }

    [Theory]
    [InlineData(30, 20, 10)]
    [InlineData(10, 30, 20)]
    [InlineData(20, 10, 30)]
    public void MaxDimensionReturnsLargestDimensionRegardlessOfOrientation(
        int width, int height, int depth)
    {
        var parcel = new Parcel(width, height, depth);

        Assert.Equal(30, parcel.MaxDimension);
    }
}
