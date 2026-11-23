using FirstAML.Models;

namespace FirstAML.Tests.Models;

public class ParcelTests
{
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
